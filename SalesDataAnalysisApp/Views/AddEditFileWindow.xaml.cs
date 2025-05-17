using Microsoft.Win32;
using System;
using System.Windows;
using SalesDataAnalysisApp.Models;
using SalesDataAnalysisApp.FileManagement;
using SalesDataAnalysisApp.Users;

namespace SalesDataAnalysisApp
{
    public partial class AddEditFileWindow : Window
    {
        private readonly FileService _fileService;
        private readonly User _currentUser;
        private readonly int _archiveId;
        private File _file;
        private string _selectedFilePath;
        private byte[] _fileBytes;

        public AddEditFileWindow(File file, User currentUser, int archiveId)
        {
            InitializeComponent();
            _fileService = new FileService("Server=95.154.107.102;Database=SalesDataMiksist;Uid=student;Pwd=student;");
            _currentUser = currentUser;
            _archiveId = archiveId;
            _file = file ?? new File();

            if (!string.IsNullOrEmpty(_file.Name))
            {
                FileNameTextBox.Text = _file.Name;
                CategoryTextBox.Text = _file.Category;
                PriorityTextBox.Text = _file.Priority.ToString();
            }
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Документы (*.docx;*.pdf)|*.docx;*.pdf|Все файлы (*.*)|*.*",
                Title = "Выберите файл"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _selectedFilePath = openFileDialog.FileName;
                FileNameTextBox.Text = System.IO.Path.GetFileName(_selectedFilePath);
                _fileBytes = System.IO.File.ReadAllBytes(_selectedFilePath);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedFilePath) || _fileBytes == null)
            {
                MessageBox.Show("Выберите .docx файл для загрузки.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(PriorityTextBox.Text, out int priority))
            {
                MessageBox.Show("Приоритет должен быть числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _file.Name = FileNameTextBox.Text;
            _file.OwnerId = _currentUser.Id;
            _file.CreatedDate = DateTime.Now;
            _file.Priority = priority;
            _file.Category = CategoryTextBox.Text;
            _file.Content = _fileBytes;
            _file.ArchiveId = _archiveId;

            try
            {
                if (_file.Id != 0)
                    _fileService.UpdateFile(_file, _currentUser); 
                else
                    _fileService.UploadFile(_file, _currentUser); 

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении файла: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
