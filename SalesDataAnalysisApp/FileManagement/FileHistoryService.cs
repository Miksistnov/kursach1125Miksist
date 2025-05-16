using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace SalesDataAnalysisApp.FileManagement
{
    public class FileHistoryService
    {
        private readonly string _connectionString;

        public FileHistoryService(string connectionString) => _connectionString = connectionString;

        public void LogChange(int fileId, int userId, string action, string changes)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            var cmd = new MySqlCommand(
                "INSERT INTO FileHistory (FileId, UserId, Action, Changes) VALUES (@FileId, @UserId, @Action, @Changes)",
                connection);
            cmd.Parameters.AddWithValue("@FileId", fileId);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Action", action);
            cmd.Parameters.AddWithValue("@Changes", changes);
            cmd.ExecuteNonQuery();
        }
    }
}
