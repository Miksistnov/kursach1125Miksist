using System;
using System.Collections.Generic;
using MySqlConnector;
using SalesDataAnalysisApp.Models;

namespace SalesDataAnalysisApp.FileManagement
{
    public class ReportService
    {
        private readonly string _connectionString;

        public ReportService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<File> GetFilesReport(DateTime? startDate, DateTime? endDate, string category = null)
        {
            var files = new List<File>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            string query = @"SELECT f.*, u.Username AS OwnerName, u.Role AS OwnerRole
                             FROM Files f
                             JOIN Users u ON f.OwnerId = u.Id
                             WHERE 1=1";
            if (startDate.HasValue)
                query += " AND f.CreatedDate >= @StartDate";
            if (endDate.HasValue)
                query += " AND f.CreatedDate <= @EndDate";
            if (!string.IsNullOrEmpty(category))
                query += " AND f.Category = @Category";

            using var command = new MySqlCommand(query, connection);
            if (startDate.HasValue)
                command.Parameters.AddWithValue("@StartDate", startDate.Value);
            if (endDate.HasValue)
                command.Parameters.AddWithValue("@EndDate", endDate.Value);
            if (!string.IsNullOrEmpty(category))
                command.Parameters.AddWithValue("@Category", category);

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
    }
}
