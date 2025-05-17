using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SalesDataAnalysisApp.Models;
using SalesDataAnalysisApp.FileManagement;
using SalesDataAnalysisApp.Users;
using SalesDataAnalysisApp.Notification;
using SalesDataAnalysisApp.Views;
namespace SalesDataAnalysisApp
{
    public partial class FileManagementPage : Page
    {
        private readonly string _connectionString;
        private readonly User _currentUser;
        private FileService _fileService;
        private ArchiveService _archiveService;
        private NotificationService _notificationService;
        private List<Archive> _archives = new List<Archive>();
        private List<File> _allFiles = new List<File>();

        public FileManagementPage(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _connectionString = "Server=95.154.107.102;Database=SalesDataMiksist;Uid=student;Pwd=student;";
            _fileService = new FileService(_connectionString);
            _archiveService = new ArchiveService(_connectionString);
            _notificationService = new NotificationService(_connectionString);
            SetFileActionButtonsState();

            LoadArchives();
        }
        private void SetFileActionButtonsState()
{
    bool canEdit = !_currentUser.IsBlocked;
    AddButton.IsEnabled = canEdit;
    EditButton.IsEnabled = canEdit;
    DeleteButton.IsEnabled = canEdit;
    DeleteSelectedButton.IsEnabled = canEdit;
    UploadEditedFileButton.IsEnabled = canEdit;
}


        private void LoadArchives()
        {
            _archives = _archiveService.GetAllArchives();
            ArchiveListBox.ItemsSource = null;
            ArchiveListBox.ItemsSource = _archives;
        }

        private void ArchiveListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ArchiveListBox.SelectedItem is Archive archive)
            {
                LoadFilesByArchive(archive.Id);
            }
            else
            {
                FilesListView.ItemsSource = null;
            }
        }

        private void LoadFilesByArchive(int archiveId)
        {
            _allFiles = _fileService.GetFilesByArchive(archiveId);
            FilesListView.ItemsSource = _allFiles;
            UpdateCategoryComboBox();
        }

        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            string filter = FilterTextBox.Text?.ToLower() ?? "";
            string selectedCategory = CategoryComboBox.SelectedItem as string;

            var filtered = _allFiles.Where(f =>
                (string.IsNullOrEmpty(filter) ||
                    (f.Name?.ToLower().Contains(filter) ?? false) ||
                    (f.Category?.ToLower().Contains(filter) ?? false))
                && (string.IsNullOrEmpty(selectedCategory) || selectedCategory == "Все категории" || f.Category == selectedCategory)
            ).ToList();

            FilesListView.ItemsSource = filtered;
        }

        private void UpdateCategoryComboBox()
        {
            var categories = _allFiles.Select(f => f.Category).Distinct().ToList();
            categories.Insert(0, "Все категории");
            CategoryComboBox.ItemsSource = categories;
            CategoryComboBox.SelectedIndex = 0;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (ArchiveListBox.SelectedItem is Archive archive)
            {
                var addWindow = new AddEditFileWindow(null, _currentUser, archive.Id);
                if (addWindow.ShowDialog() == true)
                {
                    LoadFilesByArchive(archive.Id);

                    if (_currentUser.Role == Role.Admin)
                    {
                        MessageBox.Show("Файл добавлен без проверки (вы администратор).", "Информация");
                    }
                    else
                    {
                      
                        var mods = GetModeratorsAndAdmins();
                        foreach (var mod in mods)
                            _notificationService.SendNotification(mod.Id, $"Пользователь {_currentUser.Username} отправил файл на модерацию.");
                        MessageBox.Show("Файл отправлен на модерацию.", "Информация");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите архив для создания файла.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private List<User> GetModeratorsAndAdmins()
        {
            var users = new List<User>();
            using (var connection = new MySqlConnector.MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Users WHERE Role = 'Moderator' OR Role = 'Admin'";
                using (var command = new MySqlConnector.MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = reader.GetInt32("Id"),
                            Username = reader.GetString("Username"),
                            Role = (Role)Enum.Parse(typeof(Role), reader.GetString("Role")),
                            IsBlocked = reader.GetBoolean("IsBlocked")
                        });
                    }
                }
            }
            return users;
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (FilesListView.SelectedItem is File selectedFile)
            {
                string tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), selectedFile.Name);
                System.IO.File.WriteAllBytes(tempPath, selectedFile.Content);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = tempPath,
                    UseShellExecute = true
                });

                MessageBox.Show("После редактирования сохраните файл и используйте 'Загрузить изменённый файл' для обновления.", "Инструкция", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void ModerationButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser.Role == Role.Moderator || _currentUser.Role == Role.Admin)
            {
                var moderationWindow = new ModerationWindow(_fileService, _notificationService, _currentUser);
                moderationWindow.ShowDialog();
                LoadArchives();
            }
            else
            {
                MessageBox.Show("Доступно только модераторам и администраторам.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UploadEditedFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (FilesListView.SelectedItem is File selectedFile)
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Документы (*.docx;*.pdf)|*.docx;*.pdf|Все файлы (*.*)|*.*",
                    Title = "Выберите файл"
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    byte[] newContent = System.IO.File.ReadAllBytes(openFileDialog.FileName);
                    _fileService.UpdateFileContent(selectedFile.Id, newContent);
                    MessageBox.Show("Файл успешно обновлён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadFilesByArchive(selectedFile.ArchiveId);
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ArchiveListBox.SelectedItem is Archive archive && FilesListView.SelectedItem is File selectedFile)
            {
                var editWindow = new AddEditFileWindow(selectedFile, _currentUser, archive.Id);
                if (editWindow.ShowDialog() == true)
                {
                    LoadFilesByArchive(archive.Id);
                }
            }
            else
            {
                MessageBox.Show("Выберите файл для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ArchiveListBox.SelectedItem is Archive archive && FilesListView.SelectedItem is File selectedFile)
            {
                if (_fileService.DeleteFile(selectedFile.Id, _currentUser))
                {
                    LoadFilesByArchive(archive.Id);
                }
                else
                {
                    MessageBox.Show("Недостаточно прав для удаления этого файла!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите файл для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedFiles = FilesListView.SelectedItems.Cast<File>().ToList();
            if (selectedFiles.Count == 0)
            {
                MessageBox.Show("Выберите файлы для удаления.");
                return;
            }

            if (MessageBox.Show($"Удалить {selectedFiles.Count} файлов?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                foreach (var file in selectedFiles)
                {
                    _fileService.DeleteFile(file.Id, _currentUser);
                }
                if (ArchiveListBox.SelectedItem is Archive archive)
                    LoadFilesByArchive(archive.Id);
            }
        }

    }
}
