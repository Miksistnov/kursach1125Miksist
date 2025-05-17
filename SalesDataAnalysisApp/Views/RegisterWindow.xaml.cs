using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SalesDataAnalysisApp.Users;

namespace SalesDataAnalysisApp
{
    public partial class RegisterWindow : Window
    {
        private Users.RegisterUser registerUser = new Users.RegisterUser();

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterStatus.Text = "";
            string username = RegisterUsernameTextBox.Text.Trim();
            string password = RegisterPasswordBox.Password;
            string roleStr = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "User";
            Role role = Enum.TryParse(roleStr, out Role parsedRole) ? parsedRole : Role.User;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                RegisterStatus.Text = "Заполните все поля!";
                return;
            }

            try
            {
                registerUser.Register(username, password, role);
                MessageBox.Show("Регистрация прошла успешно!");
                this.Close();
            }
            catch (Exception ex)
            {
                RegisterStatus.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RegisterPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(RegisterPasswordBox.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void RegisterUsernameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            UsernamePlaceholder.Visibility = Visibility.Collapsed;
        }

        private void RegisterUsernameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UsernamePlaceholder.Visibility = string.IsNullOrEmpty(RegisterUsernameTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void RegisterUsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UsernamePlaceholder.Visibility = string.IsNullOrEmpty(RegisterUsernameTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }
}
