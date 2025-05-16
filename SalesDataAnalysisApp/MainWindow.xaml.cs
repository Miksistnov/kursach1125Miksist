using System.Windows;
using System.Windows.Controls;
using SalesDataAnalysisApp.Users;
using System.Net.NetworkInformation;
using System.Windows.Media;
using SalesDataAnalysisApp.Model;

namespace SalesDataAnalysisApp
{
    public partial class MainWindow : Window
    {
        private readonly Database _db = new Database();
        private bool _isConnectionChecked = false;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            // Настройка визуальных эффектов
            this.Effect = new System.Windows.Media.Effects.DropShadowEffect
            {
                Color = Colors.Black,
                Direction = 320,
                ShadowDepth = 10,
                Opacity = 0.3
            };
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isConnectionChecked)
            {
                await CheckDatabaseConnectionAsync();
                _isConnectionChecked = true;
            }

            // Установка фокуса на поле ввода логина
            UsernameTextBox.Focus();
        }

        private async Task CheckDatabaseConnectionAsync()
        {
            try
            {
                LoginStatus.Text = "Проверка подключения к серверу...";
                LoginStatus.Visibility = Visibility.Visible;

                bool isConnected = await Task.Run(() => _db.TestConnection());

                if (!isConnected)
                {
                    LoginStatus.Text = "Ошибка подключения к серверу!";
                    LoginButton.IsEnabled = false;
                }
                else
                {
                    LoginStatus.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                LoginStatus.Text = "Критическая ошибка подключения!";
                LoginButton.IsEnabled = false;
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ShowErrorMessage("Введите имя пользователя и пароль");
                return;
            }

            try
            {
                LoginButton.Content = "Вход...";
                LoginButton.IsEnabled = false;

                // Имитация задержки для UX
                await Task.Delay(500);

                var user = await Task.Run(() => _db.Login(username, password));

                if (user != null)
                {
                    AppState.CurrentUser = user;

                    // Запуск главного окна
                    var mainHub = new MainHubWindow();
                    mainHub.Show();

                    // Закрытие текущего окна
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
            finally
            {
                LoginButton.Content = "Войти";
                LoginButton.IsEnabled = true;
            }
        }

        private void ShowErrorMessage(string message)
        {
            LoginStatus.Text = message;
            LoginStatus.Visibility = Visibility.Visible;

            // Анимация появления ошибки
            var animation = new System.Windows.Media.Animation.DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.3)
            };
            LoginStatus.BeginAnimation(OpacityProperty, animation);
        }

        private void OpenRegisterWindow_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            registerWindow.ShowDialog();
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Функция восстановления пароля временно недоступна",
                          "Восстановление пароля",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Tag != null)
            {
                if (textBox.Text == textBox.Tag.ToString())
                {
                    textBox.Text = string.Empty;
                    textBox.Foreground = Brushes.Black;
                }
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Tag != null)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = textBox.Tag.ToString();
                    textBox.Foreground = Brushes.Gray;
                }
            }
        }
    }
}