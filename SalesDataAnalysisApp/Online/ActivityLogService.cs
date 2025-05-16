using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace SalesDataAnalysisApp.Online
{
    public class ActivityLogService
    {
        private readonly string _connectionString;

        public ActivityLogService(string connectionString) => _connectionString = connectionString;

        public void LogActivity(int userId, string actionType, string details)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            var cmd = new MySqlCommand(
                "INSERT INTO ActivityLogs (UserId, ActionType, Details) VALUES (@UserId, @ActionType, @Details)",
                connection);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@ActionType", actionType);
            cmd.Parameters.AddWithValue("@Details", details);
            cmd.ExecuteNonQuery();
        }
    }
}
