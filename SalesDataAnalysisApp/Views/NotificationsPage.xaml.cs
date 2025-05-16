using System.Windows.Controls;
using System.Collections.Generic;
using SalesDataAnalysisApp.Model;
using NotificationModel = SalesDataAnalysisApp.Notification.Notification; 
namespace SalesDataAnalysisApp
{
    public partial class NotificationsPage : Page
    {
        private readonly Notification.NotificationService _notificationService =
            new Notification.NotificationService("Server=95.154.107.102;Database=SalesDataMiksist;Uid=student;Pwd=student;");

        public NotificationsPage()
        {
            InitializeComponent();
            Loaded += (s, e) => LoadNotifications();
        }

        private void LoadNotifications()
        {
            List<NotificationModel> notifications = _notificationService.GetNotifications(AppState.CurrentUser.Id);
            NotificationsList.ItemsSource = notifications;
        }
    }
}