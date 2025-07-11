﻿using System;
using System.Collections.Generic;
using MySqlConnector;

namespace SalesDataAnalysisApp.Notification
{
    public class NotificationService
    {
        private readonly string _connectionString;

        public NotificationService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SendNotification(int userId, string message)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            string query = @"
                INSERT INTO Notifications (UserId, Message, Timestamp, IsRead) 
                VALUES (@UserId, @Message, @Timestamp, @IsRead)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@Message", message);
            command.Parameters.AddWithValue("@Timestamp", DateTime.Now);
            command.Parameters.AddWithValue("@IsRead", false);
            command.ExecuteNonQuery();
        }
        public void MarkAsRead(int notificationId)
        {
            using var connection = new MySqlConnector.MySqlConnection(_connectionString);
            connection.Open();
            var command = new MySqlConnector.MySqlCommand(
                "UPDATE Notifications SET IsRead = true WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", notificationId);
            command.ExecuteNonQuery();
        }
        public void ClearNotifications(int userId)
        {
            using var connection = new MySqlConnector.MySqlConnection(_connectionString);
            connection.Open();
            var command = new MySqlConnector.MySqlCommand("DELETE FROM Notifications WHERE UserId = @UserId", connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.ExecuteNonQuery();
        }

        public List<Notification> GetNotifications(int userId)
        {
            var notifications = new List<Notification>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            var command = new MySqlCommand("SELECT * FROM Notifications WHERE UserId = @UserId ORDER BY Timestamp DESC", connection);
            command.Parameters.AddWithValue("@UserId", userId);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                notifications.Add(new Notification
                {
                    Id = reader.GetInt32("Id"),
                    UserId = reader.GetInt32("UserId"),
                    Message = reader.GetString("Message"),
                    Timestamp = reader.GetDateTime("Timestamp"),
                    IsRead = reader.GetBoolean("IsRead")
                });
            }
            return notifications;
        }
    }
}
