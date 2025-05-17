using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using SalesDataAnalysisApp.Model;
using NotificationModel = SalesDataAnalysisApp.Notification.Notification;

namespace SalesDataAnalysisApp
{
    public partial class NotificationsPage : Page
    {
        private readonly Notification.NotificationService _notificationService =
            new Notification.NotificationService("Server=95.154.107.102;Database=SalesDataMiksist;Uid=student;Pwd=student;");
        private List<NotificationModel> _allNotifications;

        public NotificationsPage()
        {
            InitializeComponent();
            Loaded += (s, e) => LoadNotifications();
        }

        private void LoadNotifications()
        {
            _allNotifications = _notificationService.GetNotifications(AppState.CurrentUser.Id);
            NotificationsList.ItemsSource = _allNotifications;
        }

        private void ShowAll_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NotificationsList.ItemsSource = _allNotifications;
        }

        private void ShowUnread_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NotificationsList.ItemsSource = _allNotifications.Where(n => !n.IsRead).ToList();
        }

        private void ShowRead_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NotificationsList.ItemsSource = _allNotifications.Where(n => n.IsRead).ToList();
        }

        private void Refresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadNotifications();
        }

        private void MarkAsRead_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (NotificationsList.SelectedItem is NotificationModel notification && !notification.IsRead)
            {
                _notificationService.MarkAsRead(notification.Id);
                LoadNotifications();
            }
        }
    }
}
