using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace SalesDataAnalysisApp.Users
{
    public class UserService
    {
        private readonly string _connectionString;

        public UserService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public User Login(string username, string password)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            var command = new MySqlCommand("SELECT * FROM Users WHERE Username = @Username", connection);
            command.Parameters.AddWithValue("@Username", username);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var storedPassword = reader["Password"].ToString();
                var isBlocked = Convert.ToBoolean(reader["IsBlocked"]);

                if (isBlocked)
                {
                    throw new Exception("Ваш аккаунт заблокирован.");
                }

                if (password == storedPassword)
                {
                    return new User
                    {
                        Id = reader.GetInt32("Id"),
                        Username = reader.GetString("Username"),
                        Role = (Role)Enum.Parse(typeof(Role), reader.GetString("Role")),
                        IsBlocked = isBlocked
                    };
                }
                else
                {
                    throw new Exception("Неверное имя пользователя или пароль.");
                }
            }

            throw new Exception("Пользователь не найден.");
        }
        public void BlockUser(int userId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var command = new MySqlCommand("UPDATE Users SET IsBlocked = true WHERE Id = @UserId", connection);
                command.Parameters.AddWithValue("@UserId", userId);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteUser(int userId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var command = new MySqlCommand("UPDATE Users SET IsBlocked = true WHERE Id = @UserId", connection);
                command.Parameters.AddWithValue("@UserId", userId);
                command.ExecuteNonQuery();
            }
        }


    }

}
