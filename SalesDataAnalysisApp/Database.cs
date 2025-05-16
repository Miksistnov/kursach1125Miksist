using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySqlConnector;
using SalesDataAnalysisApp.Users;

namespace SalesDataAnalysisApp
{
    public class Database
    {
        private readonly string connectionString = "Server=95.154.107.102;Database=SalesDataMiksist;Uid=student;Pwd=student;";

        public User Login(string username, string password)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = new MySqlCommand(
                    "SELECT Id, Username, Role FROM Users WHERE Username = @Username AND Password = @Password",
                    connection);

                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = reader.GetInt32("Id"),
                            Username = reader.GetString("Username"),
                            Role = (Role)Enum.Parse(typeof(Role), reader.GetString("Role"))
                        };
                    }
                   
                }
            }
            MessageBox.Show("Неверные учетные данные", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return null;

        }

        public bool TestConnection()
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }

}
