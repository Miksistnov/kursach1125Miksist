using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using SalesDataAnalysisApp.Models;
using SalesDataAnalysisApp.FileManagement;
using SalesDataAnalysisApp.Users;

namespace SalesDataAnalysisApp
{
    public partial class FileManagementPage : Page
    {
        private readonly string _connectionString;
        private readonly User _currentUser;
        private FileService _fileService;
        private ArchiveService _archiveService;
        private List<Archive> _archives = new List<Archive>();

        public FileManagementPage(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _connectionString = "Server=95.154.107.102;Database=SalesDataMiksist;Uid=student;Pwd=student;";
            _fileService = new FileService(_connectionString);
            _archiveService = new ArchiveService(_connectionString);

            LoadArchives();
        }

        private void LoadArchives()
        {
            _archives = _archiveService.GetAllArchives();
            ArchiveListBox.ItemsSource = _archives;
            if (_archives.Count > 0)
                ArchiveListBox.SelectedIndex = 0;
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
            var files = _fileService.GetFilesByArchive(archiveId);
            FilesListView.ItemsSource = files;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (ArchiveListBox.SelectedItem is Archive archive)
            {
                var newFile = new File
                {
                    Name = "Новый файл",
                    OwnerId = _currentUser.Id,
                    CreatedDate = System.DateTime.Now,
                    Priority = 1,
                    Category = "Общее",
                    Content = new byte[0], 
                    ArchiveId = archive.Id
                };


                var addWindow = new AddEditFileWindow(newFile, _currentUser, archive.Id);
                if (addWindow.ShowDialog() == true)
                {
                    LoadFilesByArchive(archive.Id);
                }
            }
            else
            {
                MessageBox.Show("Выберите архив для создания файла.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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

        private void UploadEditedFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (FilesListView.SelectedItem is File selectedFile)
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Word документы (*.docx)|*.docx",
                    Title = "Выберите изменённый файл"
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
    }
}
