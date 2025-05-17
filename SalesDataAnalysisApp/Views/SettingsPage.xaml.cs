using System.Windows;
using System.Windows.Controls;
using SalesDataAnalysisApp.Model;
using SalesDataAnalysisApp.Users;
using SalesDataAnalysisApp.Notification;
using SalesDataAnalysisApp.Views;
namespace SalesDataAnalysisApp.Views
{
    public partial class SettingsPage : Page
    {
        private readonly string _connectionString;
        private readonly UserService _userService;
        private readonly NotificationService _notificationService;
        public bool IsAdmin { get; set; }

        public SettingsPage()
        {
            InitializeComponent();
            _userService = new UserService("Server=95.154.107.102;Database=SalesDataMiksist;Uid=student;Pwd=student;");
            _notificationService = new NotificationService("Server=95.154.107.102;Database=SalesDataMiksist;Uid=student;Pwd=student;");
            IsAdmin = AppState.CurrentUser?.Role == Role.Admin;
            _connectionString = "Server=95.154.107.102;Database=SalesDataMiksist;Uid=student;Pwd=student;";
            DataContext = this;
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            var oldPass = OldPasswordBox.Password;
            var newPass = NewPasswordBox.Password;
            var repeatPass = RepeatPasswordBox.Password;
            var user = AppState.CurrentUser;

            if (user == null)
            {
                MessageBox.Show("Ошибка: пользователь не найден.");
                return;
            }

            if (string.IsNullOrWhiteSpace(newPass) || newPass != repeatPass)
            {
                MessageBox.Show("Пароли не совпадают или пусты.");
                return;
            }

            var userService = new UserService("Server=95.154.107.102;Database=SalesDataMiksist;Uid=student;Pwd=student;");
            try
            {
               
                userService.Login(user.Username, oldPass);
            }
            catch
            {
                MessageBox.Show("Старый пароль неверен.");
                return;
            }

            try
            {
                userService.ChangePassword(user.Id, newPass);
                MessageBox.Show("Пароль успешно изменён!");
                OldPasswordBox.Clear();
                NewPasswordBox.Clear();
                RepeatPasswordBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при смене пароля: " + ex.Message);
            }
        }


        private void ShowNotifications_Click(object sender, RoutedEventArgs e)
        {
            var user = AppState.CurrentUser;
            if (user == null) return;
            var notifications = _notificationService.GetNotifications(user.Id);
            string msg = notifications.Count == 0
                ? "Нет уведомлений."
                : string.Join("\n\n", notifications.ConvertAll(n =>
                    $"{n.Timestamp:G}\n{n.Message}"));
            MessageBox.Show(msg, "Уведомления");
        }

    
        private void ClearNotifications_Click(object sender, RoutedEventArgs e)
        {
            var user = AppState.CurrentUser;
            if (user == null) return; 
            _notificationService.ClearNotifications(user.Id);
            MessageBox.Show("Уведомления очищены.");
        }

        private void BlockUser_Click(object sender, RoutedEventArgs e)
        {
            var userService = new UserService(_connectionString);
            var users = userService.GetAllUsers()
                .Where(u => u.Id != AppState.CurrentUser.Id) 
                .ToList();

            var window = new UserSelectWindow(users);
            if (window.ShowDialog() == true && window.SelectedUser != null)
            {
                userService.BlockUser(window.SelectedUser.Id);
                MessageBox.Show($"Пользователь {window.SelectedUser.Username} заблокирован.");
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var userService = new UserService(_connectionString);
            var users = userService.GetAllUsers()
                .Where(u => u.Id != AppState.CurrentUser.Id)
                .ToList();

            var window = new UserSelectWindow(users);
            if (window.ShowDialog() == true && window.SelectedUser != null)
            {
                userService.DeleteUser(window.SelectedUser.Id);
                MessageBox.Show($"Пользователь {window.SelectedUser.Username} удалён.");
            }
        }

        private void UnblockUser_Click(object sender, RoutedEventArgs e)
        {
            var userService = new UserService(_connectionString);
            var users = userService.GetAllUsers()
                .Where(u => u.IsBlocked)
                .ToList();

            var window = new UserSelectWindow(users);
            if (window.ShowDialog() == true && window.SelectedUser != null)
            {
                userService.UnblockUser(window.SelectedUser.Id);
                MessageBox.Show($"Пользователь {window.SelectedUser.Username} разблокирован.");
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            AppState.CurrentUser = null;
            MessageBox.Show("Вы вышли из аккаунта.");
            {
                var loginWindow = new MainWindow();
                loginWindow.Show();
                Window.GetWindow(this)?.Close();
            }
        }
    }
}
