using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SalesDataAnalysisApp.Users;
using SalesDataAnalysisApp.Views;
namespace SalesDataAnalysisApp.Views
{
    public partial class UserSelectWindow : Window
    {
        private List<User> _allUsers;
        public User SelectedUser { get; private set; }

        public UserSelectWindow(List<User> users)
        {
            InitializeComponent();
            _allUsers = users;
            UsersListBox.ItemsSource = _allUsers;
        }

        private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var filter = SearchBox.Text.ToLower();
            UsersListBox.ItemsSource = string.IsNullOrWhiteSpace(filter)
                ? _allUsers
                : _allUsers.Where(u => u.Username.ToLower().Contains(filter)).ToList();
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListBox.SelectedItem is User user)
            {
                SelectedUser = user;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Выберите пользователя.");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
