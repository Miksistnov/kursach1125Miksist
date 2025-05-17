using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace SalesDataAnalysisApp.Users
{
    public class RegisterUser
    {
        private string connectionString = "Server=95.154.107.102;Database=SalesDataMiksist;Uid=student;Pwd=student;";

        public void Register(string username, string password, Role role)
        {
            if (IsUsernameTaken(username))
            {
                throw new Exception("Имя пользователя уже занято.");
            }

            //

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Users (Username, Password, Role, IsBlocked) VALUES (@Username, @Password, @Role, @IsBlocked)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Role", role.ToString());
                    command.Parameters.AddWithValue("@IsBlocked", false);
                    command.ExecuteNonQuery();
                }
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
    }

}
