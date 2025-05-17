using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using MySqlConnector;

namespace SalesDataAnalysisApp.Views
{
    /// <summary>
    /// Логика взаимодействия для ReportGenerator.xaml
    /// </summary>
    public partial class ReportGenerator : Window
    {
        private readonly string _connectionString = "Server=95.154.107.102;Database=SalesDataMiksist;Uid=student;Pwd=student;";

        public ReportGenerator(string connectionString)
        {
            InitializeComponent();
            _connectionString = connectionString;
        }
        public DataTable GenerateFilesReport(DateTime startDate, DateTime endDate, int? categoryId = null)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            string query = @"SELECT f.Name, f.CreatedDate, u.Username as Owner, c.Name as Category 
                        FROM Files f
                        JOIN Users u ON f.OwnerId = u.Id
                        JOIN Categories c ON f.CategoryId = c.Id
                        WHERE f.CreatedDate BETWEEN @StartDate AND @EndDate";

            if (categoryId.HasValue)
            {
                query += " AND f.CategoryId = @CategoryId";
            }

            query += " ORDER BY f.CreatedDate DESC";

            var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            if (categoryId.HasValue)
            {
                command.Parameters.AddWithValue("@CategoryId", categoryId.Value);
            }

            var adapter = new MySqlDataAdapter(command);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);

            return dataTable;
        }

        // Метод для создания отчета по активности пользователей
        public DataTable GenerateUserActivityReport()
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            string query = @"SELECT u.Username, 
                        COUNT(f.Id) as FilesCount, 
                        MAX(f.CreatedDate) as LastActivity
                        FROM Users u
                        LEFT JOIN Files f ON u.Id = f.OwnerId
                        GROUP BY u.Id, u.Username
                        ORDER BY FilesCount DESC";

            var command = new MySqlCommand(query, connection);
            var adapter = new MySqlDataAdapter(command);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);

            return dataTable;
        }

        // Метод для экспорта отчета в Excel
        public void ExportToExcel(DataTable dataTable, string filePath)
        {
            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Report");

            // Добавляем заголовки
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                worksheet.Cell(1, i + 1).Value = dataTable.Columns[i].ColumnName;
            }

            // Добавляем данные
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    worksheet.Cell(i + 2, j + 1).Value = dataTable.Rows[i][j].ToString();
                }
            }

            workbook.SaveAs(filePath);
        }

        // Метод для экспорта отчета в PDF
        public void ExportToPdf(DataTable dataTable, string filePath)
        {
            using var document = new iText.Kernel.Pdf.PdfDocument(
                new iText.Kernel.Pdf.PdfWriter(filePath));
            using var pdf = new iText.Layout.Document(document);

            // Создаем таблицу в PDF
            var table = new iText.Layout.Element.Table(dataTable.Columns.Count);

            // Добавляем заголовки
            foreach (DataColumn column in dataTable.Columns)
            {
                table.AddHeaderCell(column.ColumnName);
            }

            // Добавляем данные
            foreach (DataRow row in dataTable.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    table.AddCell(item.ToString());
                }
            }

            pdf.Add(table);
        }

        private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var startDate = StartDatePicker.SelectedDate ?? DateTime.Now.AddMonths(-1);
                var endDate = EndDatePicker.SelectedDate ?? DateTime.Now;
                int? categoryId = CategoryComboBox.SelectedValue as int?;

                var reportGenerator = new ReportGenerator(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
                var reportData = reportGenerator.GenerateFilesReport(startDate, endDate, categoryId);

                // Отображаем данные в DataGrid
                ReportDataGrid.ItemsSource = reportData.DefaultView;

                // Предлагаем экспорт
                if (MessageBox.Show("Отчет сформирован. Экспортировать в Excel?", "Экспорт",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var saveDialog = new SaveFileDialog
                    {
                        Filter = "Excel Files (*.xlsx)|*.xlsx",
                        DefaultExt = "xlsx",
                        FileName = $"Report_{DateTime.Now:yyyyMMdd}"
                    };

                    if (saveDialog.ShowDialog() == true)
                    {
                        reportGenerator.ExportToExcel(reportData, saveDialog.FileName);
                        MessageBox.Show($"Отчет успешно экспортирован в {saveDialog.FileName}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании отчета: {ex.Message}");
            }
        }
    }
}
