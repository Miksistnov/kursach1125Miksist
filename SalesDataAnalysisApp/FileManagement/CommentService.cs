using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using SalesDataAnalysisApp.Model;

namespace SalesDataAnalysisApp.FileManagement
{
    public class CommentService
    {
        private readonly string _connectionString;

        public CommentService(string connectionString) => _connectionString = connectionString;

        public void AddComment(Comment comment)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            var cmd = new MySqlCommand(
                "INSERT INTO Comments (FileId, UserId, Text) VALUES (@FileId, @UserId, @Text)",
                connection);
            cmd.Parameters.AddWithValue("@FileId", comment.FileId);
            cmd.Parameters.AddWithValue("@UserId", comment.UserId);
            cmd.Parameters.AddWithValue("@Text", comment.Text);
            cmd.ExecuteNonQuery();
        }
    }
}
