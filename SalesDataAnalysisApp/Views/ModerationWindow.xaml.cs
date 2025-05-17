using System.Windows;
using SalesDataAnalysisApp.Models;
using SalesDataAnalysisApp.FileManagement;
using SalesDataAnalysisApp.Notification;
using SalesDataAnalysisApp.Users;
using System.Collections.Generic;
using SalesDataAnalysisApp.Model;

namespace SalesDataAnalysisApp.Views
{
    public partial class ModerationWindow : Window
    {
        private readonly FileService _fileService;
        private readonly NotificationService _notificationService;
        private readonly User _moderator;

        private List<File> _pendingFiles;

        public ModerationWindow(FileService fileService, NotificationService notificationService, User moderator)
        {
            InitializeComponent();
            _fileService = fileService;
            _notificationService = notificationService ?? new NotificationService("Server=95.154.107.102;Database=SalesDataMiksist;Uid=student;Pwd=student;");
            _moderator = moderator;
            LoadPendingFiles();
        }

        private void ApproveSelected_Click(object sender, RoutedEventArgs e)
        {
            var selectedFiles = PendingFilesGrid.SelectedItems.Cast<File>().ToList();
            if (selectedFiles.Count == 0)
            {
                MessageBox.Show("Выберите файлы для модерации.");
                return;
            }

            var moderator = AppState.CurrentUser;
            foreach (var file in selectedFiles)
            {
                _fileService.ModerateFile(file.Id, moderator.Id, "Approved", "");
                _notificationService.SendNotification(file.OwnerId, $"Ваш файл \"{file.Name}\" одобрен модератором.");
            }
            MessageBox.Show($"Одобрено файлов: {selectedFiles.Count}");
            LoadPendingFiles();
        }


        private void LoadPendingFiles()
        {
            _pendingFiles = _fileService.GetPendingFiles();
            PendingFilesGrid.ItemsSource = _pendingFiles;
        }

        private void Approve_Click(object sender, RoutedEventArgs e)
        {
            if (PendingFilesGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите файл для модерации.");
                return;
            }

            if (PendingFilesGrid.SelectedItem is File file)
            {
                _fileService.ModerateFile(file.Id, _moderator.Id, "Approved", "");
                _notificationService.SendNotification(file.OwnerId, $"Ваш файл \"{file.Name}\" одобрен модератором.");
                LoadPendingFiles();
            }
        }
        private void Reject_Click(object sender, RoutedEventArgs e)
        {
            if (PendingFilesGrid.SelectedItem is File file)
            {
                var comment = Microsoft.VisualBasic.Interaction.InputBox("Введите причину отклонения:", "Отклонить файл", ""); ; 
                _fileService.ModerateFile(file.Id, _moderator.Id, "Rejected", comment);
                _notificationService.SendNotification(file.OwnerId, $"Ваш файл \"{file.Name}\" отклонён модератором. Причина: {comment}");
                LoadPendingFiles();
            }
        }
    }
}
