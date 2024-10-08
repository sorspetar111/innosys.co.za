public class BaseActivity
{
    public int ID { get; set; }
    public string Description { get; set; }
    public string Client { get; set; }
    public DateTime StartDate { get; set; }
    public int Duration { get; set; }
    public string Task1 { get; set; }
    public string Task2 { get; set; }
    public string Task3 { get; set; }
    public string Task4 { get; set; }
    public string Task5 { get; set; }
}


public class Activity:BaseActivity
{
    public DateTime DueDate { get; set; }  
}



using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

public class CsvParser
{
    public List<Activity> ParseCsv(string filePath)
    {
        var activities = new List<Activity>();

        using (var reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var values = line.Split(',');

                var activity = new Activity
                {
                    ID = int.Parse(values[0]),
                    Description = values[1],
                    Client = values[2],
                    StartDate = DateTime.ParseExact(values[3], "yyyyMMddTHHmm", CultureInfo.InvariantCulture),
                    Duration = int.Parse(values[4]),
                    Task1 = values[5],
                    Task2 = values[6],
                    Task3 = values[7],
                    Task4 = values[8],
                    Task5 = values[9]
                };

                activities.Add(activity);
            }
        }

        return activities;
    }
}

using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;

public class CsvParser
{
    public List<Activity> ParseCsv2(string filePath)
    {
        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Configuration.HasHeaderRecord = false;

            var records = csv.GetRecords<Activity>().ToList();
            return records;
        }
    }
}



using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class DatabaseHandler
{
    private readonly string _connectionString;

    public DatabaseHandler(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void InsertActivities(List<Activity> activities)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            foreach (var activity in activities)
            {
                using (var command = new SqlCommand("INSERT INTO Activities (ID, Description, Client, StartDate, Duration, Task1, Task2, Task3, Task4, Task5) VALUES (@ID, @Description, @Client, @StartDate, @Duration, @Task1, @Task2, @Task3, @Task4, @Task5)", connection))
                {
                    command.Parameters.AddWithValue("@ID", activity.ID);
                    command.Parameters.AddWithValue("@Description", activity.Description);
                    command.Parameters.AddWithValue("@Client", activity.Client);
                    command.Parameters.AddWithValue("@StartDate", activity.StartDate);
                    command.Parameters.AddWithValue("@Duration", activity.Duration);
                    command.Parameters.AddWithValue("@Task1", activity.Task1);
                    command.Parameters.AddWithValue("@Task2", activity.Task2);
                    command.Parameters.AddWithValue("@Task3", activity.Task3);
                    command.Parameters.AddWithValue("@Task4", activity.Task4);
                    command.Parameters.AddWithValue("@Task5", activity.Task5);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}

public class ActivityService
{
    private readonly CsvParser _csvParser;
    private readonly DatabaseHandler _databaseHandler;

    public ActivityService(CsvParser csvParser, DatabaseHandler databaseHandler)
    {
        _csvParser = csvParser;
        _databaseHandler = databaseHandler;
    }

    public void ImportActivities(string csvFilePath)
    {
        var activities = _csvParser.ParseCsv(csvFilePath);
        _databaseHandler.InsertActivities(activities);
    }
}


class Program
{
    static void Main(string[] args)
    {
        var connectionString = "YourConnectionStringHere";
        var csvParser = new CsvParser();
        var databaseHandler = new DatabaseHandler(connectionString);
        var activityService = new ActivityService(csvParser, databaseHandler);

        string csvFilePath = @"path_to_your_csv_file.csv";
        activityService.ImportActivities(csvFilePath);

        Console.WriteLine("CSV data has been successfully inserted into the database.");
    }
}




