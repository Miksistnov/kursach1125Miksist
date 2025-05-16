using MySqlConnector;
using System.Windows;
using SalesDataAnalysisApp.Models;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
namespace SalesDataAnalysisApp
{
    public partial class EditContentWindow : Window
    {
        private readonly File _file;
        private readonly string _connectionString = "Server=95.154.107.102;Database=SalesDataMiksist;Uid=student;Pwd=student;";

        public EditContentWindow(File file)
        {
            InitializeComponent();
            _file = file;
            DataContext = _file;
        }
        private void UploadImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                    ContentImage.Source = bitmap;
                    ContentImage.Visibility = Visibility.Visible;
                    ContentTextBox.Visibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
                }
            }
        }

        private void SwitchToText_Click(object sender, RoutedEventArgs e)
        {
            ContentTextBox.Visibility = Visibility.Visible;
            ContentImage.Visibility = Visibility.Collapsed;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var command = new MySqlCommand(
                    "UPDATE Files SET Content = @Content WHERE Id = @Id",
                    connection);

                command.Parameters.AddWithValue("@Content", _file.Content);
                command.Parameters.AddWithValue("@Id", _file.Id);
                command.ExecuteNonQuery();
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
