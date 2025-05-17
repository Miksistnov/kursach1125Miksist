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
        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            using var connection = new MySqlConnector.MySqlConnection(_connectionString);
            connection.Open();
            var command = new MySqlConnector.MySqlCommand("SELECT * FROM Users", connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                users.Add(new User
                {
                    Id = reader.GetInt32("Id"),
                    Username = reader.GetString("Username"),
                    Role = (Role)Enum.Parse(typeof(Role), reader.GetString("Role")),
                    IsBlocked = reader.GetBoolean("IsBlocked")
                });
            }
            return users;
        }
        public void ChangePassword(int userId, string newPassword)
        {
            using var connection = new MySqlConnector.MySqlConnection(_connectionString);
            connection.Open();
            var command = new MySqlConnector.MySqlCommand(
                "UPDATE Users SET Password = @Password WHERE Id = @UserId", connection);
            command.Parameters.AddWithValue("@Password", newPassword);
            command.Parameters.AddWithValue("@UserId", userId);
            command.ExecuteNonQuery();
        }
        public void UnblockUser(int userId)
        {
            using (var connection = new MySqlConnector.MySqlConnection(_connectionString))
            {
                connection.Open();
                var command = new MySqlConnector.MySqlCommand("UPDATE Users SET IsBlocked = false WHERE Id = @UserId", connection);
                command.Parameters.AddWithValue("@UserId", userId);
                command.ExecuteNonQuery();
            }
        }


        public List<User> GetModeratorsAndAdmins()
        {
            var users = new List<User>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            var command = new MySqlCommand("SELECT * FROM Users WHERE Role IN ('Moderator', 'Admin') AND IsBlocked = false", connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                users.Add(new User
                {
                    Id = reader.GetInt32("Id"),
                    Username = reader.GetString("Username"),
                    Role = (Role)Enum.Parse(typeof(Role), reader.GetString("Role")),
                    IsBlocked = reader.GetBoolean("IsBlocked")
                });
            }
            return users;
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
