using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MySqlConnector;
using SalesDataAnalysisApp.FileManagement;
using SalesDataAnalysisApp.Models;
using SalesDataAnalysisApp.Users;

namespace SalesDataAnalysisApp.Views
{
    public partial class FileReportPage : Page
    {
        private readonly string _connectionString;
        private readonly User _currentUser;

        public FileReportPage(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _connectionString = "Server=95.154.107.102;Database=SalesDataMiksist;Uid=student;Pwd=student;";

            LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                var categories = new List<string>();
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();
                var command = new MySqlCommand("SELECT DISTINCT Category FROM Files", connection);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    categories.Add(reader.GetString("Category"));
                }
                CategoryComboBox.ItemsSource = categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки категорий: " + ex.Message);
            }
        }

        private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var startDate = StartDatePicker.SelectedDate ?? DateTime.Now.AddMonths(-1);
                var endDate = EndDatePicker.SelectedDate ?? DateTime.Now;
                string category = CategoryComboBox.SelectedItem as string;

                var fileService = new FileService(_connectionString);
                var files = fileService.GetAllFiles();

                // Фильтрация по дате и категории
                var filtered = files.FindAll(f =>
                    f.CreatedDate >= startDate &&
                    f.CreatedDate <= endDate &&
                    (string.IsNullOrEmpty(category) || f.Category == category)
                );

                ReportDataGrid.ItemsSource = filtered;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при формировании отчёта: " + ex.Message);
            }
        }
    }
}
