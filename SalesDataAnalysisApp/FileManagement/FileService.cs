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

        public void UploadFile(File file)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            string query = "INSERT INTO Files (Name, OwnerId, CreatedDate, Priority, Category, Content, ArchiveId) VALUES (@Name, @OwnerId, @CreatedDate, @Priority, @Category, @Content, @ArchiveId)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", file.Name);
            command.Parameters.AddWithValue("@OwnerId", file.OwnerId);
            command.Parameters.AddWithValue("@CreatedDate", file.CreatedDate);
            command.Parameters.AddWithValue("@Priority", file.Priority);
            command.Parameters.AddWithValue("@Category", file.Category);
            command.Parameters.AddWithValue("@Content", file.Content ?? new byte[0]);
            command.Parameters.AddWithValue("@ArchiveId", file.ArchiveId);
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

        public bool DeleteFile(int fileId, User currentUser)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            string getRoleQuery = @"SELECT u.Role
                                    FROM Files f
                                    JOIN Users u ON f.OwnerId = u.Id
                                    WHERE f.Id = @FileId";
            using var getRoleCmd = new MySqlCommand(getRoleQuery, connection);
            getRoleCmd.Parameters.AddWithValue("@FileId", fileId);
            var ownerRoleObj = getRoleCmd.ExecuteScalar();
            if (ownerRoleObj == null)
                return false;
            string ownerRoleStr = ownerRoleObj.ToString();
            Role ownerRole = (Role)Enum.Parse(typeof(Role), ownerRoleStr);

            if ((int)currentUser.Role < (int)ownerRole)
                return false;

            string query = "DELETE FROM Files WHERE Id = @FileId";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@FileId", fileId);
            command.ExecuteNonQuery();
            return true;
        }
    }
}
