using System.Collections.Generic;
using MySqlConnector;
using SalesDataAnalysisApp.Models;

public class ArchiveService
{
    private readonly string _connectionString;
    public ArchiveService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<Archive> GetAllArchives()
    {
        var archives = new List<Archive>();
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();
        var command = new MySqlCommand("SELECT Id, Name FROM Archive", connection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            archives.Add(new Archive
            {
                Id = reader.GetInt32("Id"),
                Name = reader.GetString("Name")
            });
        }
        return archives;
    }
}
