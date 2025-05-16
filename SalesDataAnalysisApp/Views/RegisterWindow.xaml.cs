using System;
using System.Windows;
using System.Windows.Controls;
using MySqlConnector;

namespace SalesDataAnalysisApp
{
    public partial class RegisterWindow : Window
    {
        private string connectionString = "Server=95.154.107.102;Database=SalesDataMiksist;Uid=student;Pwd=student;";

        public RegisterWindow()
        {
            InitializeComponent();
        }

        public void RegisterUser(string username, string password)
        {
            if (IsUsernameTaken(username))
            {
                throw new Exception("Имя пользователя уже занято.");
            }

            string hashedPassword = HashPassword(password);

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = new MySqlCommand("INSERT INTO Users (Username, Password, Role, IsBlocked) VALUES (@Username, @Password, @Role, @IsBlocked)", connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", hashedPassword);
                command.Parameters.AddWithValue("@Role", "User "); // По умолчанию роль "User "
                command.Parameters.AddWithValue("@IsBlocked", false);
                command.ExecuteNonQuery();
            }
        }

        private bool IsUsernameTaken(string username)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = new MySqlCommand("SELECT COUNT(*) FROM Users WHERE Username = @Username", connection);
                command.Parameters.AddWithValue("@Username", username);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }

        private string HashPassword(string password)
        {
            return password;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = RegisterUsernameTextBox.Text;
                string password = RegisterPasswordBox.Password;
                string role = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "User";

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Заполните все поля!");
                    return;
                }

                if (IsUsernameTaken(username))
                {
                    MessageBox.Show("Имя пользователя уже занято!");
                    return;
                }

                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new MySqlCommand(
                        "INSERT INTO Users (Username, Password, Role, IsBlocked) VALUES(@Username, @Password, @Role, @IsBlocked)",
                        connection);

                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Role", role.Trim());
                    command.Parameters.AddWithValue("@IsBlocked", false);

                    command.ExecuteNonQuery();
                    MessageBox.Show("Регистрация прошла успешно!");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Закрыть окно регистрации
        }

        private void RegisterPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Скрываем плейсхолдер, если введен пароль
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(RegisterPasswordBox.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void PasswordPlaceholder_GotFocus(object sender, RoutedEventArgs e)
        {
            // Скрыть плейсхолдер при фокусе
            PasswordPlaceholder.Visibility = Visibility.Collapsed;
            RegisterPasswordBox.Focus();
        }

        private void PasswordPlaceholder_LostFocus(object sender, RoutedEventArgs e)
        {
            // Показать плейсхолдер, если поле пароля пустое
            if (string.IsNullOrEmpty(RegisterPasswordBox.Password))
            {
                PasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void RegisterUsernameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Скрыть плейсхолдер при фокусе
            UsernamePlaceholder.Visibility = Visibility.Collapsed; // Предполагается, что у вас есть плейсхолдер для имени пользователя
        }

        private void RegisterUsernameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Показать плейсхолдер, если поле имени пользователя пустое
            if (string.IsNullOrEmpty(RegisterUsernameTextBox.Text))
            {
                UsernamePlaceholder.Visibility = Visibility.Visible; // Предполагается, что у вас есть плейсхолдер для имени пользователя
            }
        }

        private void RegisterUsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Скрыть плейсхолдер при изменении текста
            UsernamePlaceholder.Visibility = string.IsNullOrEmpty(RegisterUsernameTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
