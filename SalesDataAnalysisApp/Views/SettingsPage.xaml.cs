using System.Windows.Controls;
using System.Windows;
using SalesDataAnalysisApp.Users;
using SalesDataAnalysisApp.Model;

namespace SalesDataAnalysisApp
{
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция смены пароля будет реализована позже");
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            AppState.CurrentUser = null;
            new MainWindow().Show();
            Window.GetWindow(this)?.Close();
        }
    }
}