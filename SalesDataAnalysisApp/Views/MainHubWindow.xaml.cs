using System.Windows;
using System.Windows.Controls;
using SalesDataAnalysisApp.Model;
using SalesDataAnalysisApp.FileManagement;
using SalesDataAnalysisApp.Views;
namespace SalesDataAnalysisApp
{
    public partial class MainHubWindow : Window
    {
        public MainHubWindow()
        {
            InitializeComponent();
            Loaded += MainHubWindow_Loaded;
        }

        private void MainHubWindow_Loaded(object sender, RoutedEventArgs e)
        {
            NavigateToPage("FileManagementPage");
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl tabControl && tabControl.SelectedItem is TabItem selectedTab)
            {
                var tag = selectedTab.Tag?.ToString();

                if (tag == "Exit")
                {
                    Application.Current.Shutdown();
                    return;
                }

                NavigateToPage(tag);
            }
        }

        private void NavigateToPage(string pageName)
        {
            try
            {
                if (MainFrame == null) return;

                switch (pageName)
                {
                    case "FileManagementPage":
                        MainFrame.Navigate(new FileManagementPage(AppState.CurrentUser));
                        break;
                    case "AnalyticsPage":
                        MainFrame.Navigate(new SalesDataAnalysisApp.Views.AnalyticsPage());
                        break;
                    case "NotificationsPage":
                        MainFrame.Navigate(new NotificationsPage());
                        break;
                    case "SettingsPage":
                        MainFrame.Navigate(new SettingsPage());
                        break;
                    case "Exit":
                        Application.Current.Shutdown();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка навигации: {ex.Message}");
            }
        }

    }
}
