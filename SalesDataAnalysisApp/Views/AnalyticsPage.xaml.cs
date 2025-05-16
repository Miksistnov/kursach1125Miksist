using LiveCharts;
using LiveCharts.Wpf;
using SalesDataAnalysisApp.FileManagement;
using System;
using System.Linq;
using System.Windows.Controls;

namespace SalesDataAnalysisApp.Views
{
    public partial class AnalyticsPage : Page
    {
        public SeriesCollection FilesPerDaySeries { get; set; }
        public string[] FilesPerDayLabels { get; set; }
        public SeriesCollection CategorySeries { get; set; }

        public AnalyticsPage()
        {
            InitializeComponent();
            DataContext = this;

            var fileService = new SalesDataAnalysisApp.FileManagement.FileService("Server=95.154.107.102;Database=SalesDataMiksist;Uid=student;Pwd=student;");
            var files = fileService.GetAllFiles();

            // Файлы по дням
            var filesPerDay = files
                .Where(f => f.CreatedDate != null)
                .GroupBy(f => f.CreatedDate.Date)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Count());

            FilesPerDaySeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Файлы",
                    Values = new ChartValues<int>(filesPerDay.Values)
                }
            };
            FilesPerDayLabels = filesPerDay.Keys.Select(d => d.ToString("dd.MM")).ToArray();

            // Популярные категории
            var categories = files
                .Where(f => !string.IsNullOrEmpty(f.Category))
                .GroupBy(f => f.Category)
                .OrderByDescending(g => g.Count())
                .ToDictionary(g => g.Key, g => g.Count());

            CategorySeries = new SeriesCollection();
            foreach (var cat in categories)
            {
                CategorySeries.Add(new PieSeries
                {
                    Title = cat.Key,
                    Values = new ChartValues<int> { cat.Value },
                    DataLabels = true
                });
            }
        }
    }
}
