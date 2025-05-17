using System;
using System.Collections.Generic;
using MySqlConnector;
using SalesDataAnalysisApp.Models;
using SalesDataAnalysisApp.Users;

namespace SalesDataAnalysisApp.FileManagement
{
    public class FileService
    {
        private readonly string _connectionString;

        public FileService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void UploadFile(File file, User currentUser)
        {
            if (currentUser.IsBlocked)
                throw new InvalidOperationException("Пользователь заблокирован и не может добавлять файлы.");

            using var connection = new MySqlConnector.MySqlConnection(_connectionString);
            connection.Open();
            string query = "INSERT INTO Files (Name, OwnerId, CreatedDate, Priority, Category, Content, ArchiveId) VALUES (@Name, @OwnerId, @CreatedDate, @Priority, @Category, @Content, @ArchiveId)";
            using var command = new MySqlConnector.MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", file.Name);
            command.Parameters.AddWithValue("@OwnerId", file.OwnerId);
            command.Parameters.AddWithValue("@CreatedDate", file.CreatedDate);
            command.Parameters.AddWithValue("@Priority", file.Priority);
            command.Parameters.AddWithValue("@Category", file.Category);
            command.Parameters.AddWithValue("@Content", file.Content ?? new byte[0]);
            command.Parameters.AddWithValue("@ArchiveId", file.ArchiveId);
            command.ExecuteNonQuery();
        }

        public void UpdateFile(File file, User currentUser)
        {
            if (currentUser.IsBlocked)
                throw new InvalidOperationException("Пользователь заблокирован и не может изменять файлы.");

            using var connection = new MySqlConnector.MySqlConnection(_connectionString);
            connection.Open();
            string query = "UPDATE Files SET Name=@Name, Priority=@Priority, Category=@Category, Content=@Content WHERE Id=@Id";
            using var command = new MySqlConnector.MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", file.Name);
            command.Parameters.AddWithValue("@Priority", file.Priority);
            command.Parameters.AddWithValue("@Category", file.Category);
            command.Parameters.AddWithValue("@Content", file.Content ??new byte[0]);
            command.Parameters.AddWithValue("@Id", file.Id);
            command.ExecuteNonQuery();
        }

        public bool DeleteFile(int fileId, User currentUser)
        {
            if (currentUser.IsBlocked)
                return false;

            using var connection = new MySqlConnector.MySqlConnection(_connectionString);
            connection.Open();
            string getRoleQuery = @"SELECT u.Role
                            FROM Files f
                            JOIN Users u ON f.OwnerId = u.Id
                            WHERE f.Id = @FileId";
            using var getRoleCmd = new MySqlConnector.MySqlCommand(getRoleQuery, connection);
            getRoleCmd.Parameters.AddWithValue("@FileId", fileId);
            var ownerRoleObj = getRoleCmd.ExecuteScalar();
            if (ownerRoleObj == null)
                return false;
            string ownerRoleStr = ownerRoleObj.ToString();
            Role ownerRole = (Role)Enum.Parse(typeof(Role), ownerRoleStr);

            if ((int)currentUser.Role < (int)ownerRole)
                return false;
            string query = "DELETE FROM Files WHERE Id = @FileId";
            using var command = new MySqlConnector.MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@FileId", fileId);
            command.ExecuteNonQuery();
            return true;
        }

        public void ApproveFile(int fileId)
        {
            using var connection = new MySqlConnector.MySqlConnection(_connectionString);
            connection.Open();
            string query = "UPDATE Files SET IsApproved = 1 WHERE Id = @FileId";
            using var command = new MySqlConnector.MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@FileId", fileId);
            command.ExecuteNonQuery();
        }

        public void ModerateFile(int fileId, int moderatorId, string status, string comment)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            string query = @"UPDATE Files SET Status = @Status, ModeratorComment = @Comment, ModeratorId = @ModeratorId WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Status", status);
            command.Parameters.AddWithValue("@Comment", comment);
            command.Parameters.AddWithValue("@ModeratorId", moderatorId);
            command.Parameters.AddWithValue("@Id", fileId);
            command.ExecuteNonQuery();
        }

        public void UpdateFileContent(int fileId, byte[] newContent)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            string query = "UPDATE Files SET Content = @Content WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Content", newContent ?? new byte[0]);
            command.Parameters.AddWithValue("@Id", fileId);
            command.ExecuteNonQuery();
        }

        public List<File> GetPendingFiles()
        {
            var files = new List<File>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            string query = @"SELECT f.*, u.Username AS OwnerName, u.Role AS OwnerRole
                     FROM Files f
                     JOIN Users u ON f.OwnerId = u.Id
                     WHERE f.Status = 'Pending'";
            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                files.Add(new File
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    OwnerId = reader.GetInt32("OwnerId"),
                    CreatedDate = reader.GetDateTime("CreatedDate"),
                    Priority = reader.GetInt32("Priority"),
                    Category = reader.GetString("Category"),
                    Content = reader["Content"] as byte[],
                    OwnerName = reader.GetString("OwnerName"),
                    OwnerRole = reader.GetString("OwnerRole"),
                    ArchiveId = reader.GetInt32("ArchiveId"),
                    Status = reader.GetString("Status"),
                    ModeratorComment = reader["ModeratorComment"] == DBNull.Value ? null : reader.GetString("ModeratorComment"),
                    ModeratorId = reader["ModeratorId"] == DBNull.Value ? (int?)null : reader.GetInt32("ModeratorId")
                });
            }
            return files;
        }
        public List<File> GetAllFiles()
        {
            var files = new List<File>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            string query = @"SELECT f.*, u.Username AS OwnerName, u.Role AS OwnerRole
                     FROM Files f
                     JOIN Users u ON f.OwnerId = u.Id";
            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                files.Add(new File
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    OwnerId = reader.GetInt32("OwnerId"),
                    CreatedDate = reader.GetDateTime("CreatedDate"),
                    Priority = reader.GetInt32("Priority"),
                    Category = reader.GetString("Category"),
                    Content = reader["Content"] as byte[],
                    OwnerName = reader.GetString("OwnerName"),
                    OwnerRole = reader.GetString("OwnerRole"),
                    ArchiveId = reader.GetInt32("ArchiveId")
                });
            }
            return files;
        }

        public List<File> GetFilesByArchive(int archiveId)
        {
            var files = new List<File>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            string query = @"SELECT f.*, u.Username AS OwnerName, u.Role AS OwnerRole
                     FROM Files f
                     JOIN Users u ON f.OwnerId = u.Id
                     WHERE f.ArchiveId = @ArchiveId";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@ArchiveId", archiveId);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                files.Add(new File
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    OwnerId = reader.GetInt32("OwnerId"),
                    CreatedDate = reader.GetDateTime("CreatedDate"),
                    Priority = reader.GetInt32("Priority"),
                    Category = reader.GetString("Category"),
                    Content = reader["Content"] as byte[],
                    OwnerName = reader.GetString("OwnerName"),
                    OwnerRole = reader.GetString("OwnerRole"),
                    ArchiveId = archiveId
                });
            }
            return files;
        }
    }
}
