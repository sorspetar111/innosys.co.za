using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class ActivityService
{
    private readonly string _connectionString;

    public ActivityService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<Activity> GetAllActivities()
    {
        var activities = new List<Activity>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("SELECT * FROM Activities", connection))
            {
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var activity = new Activity
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        Description = reader["Description"].ToString(),
                        Client = reader["Client"].ToString(),
                        StartDate = Convert.ToDateTime(reader["StartDate"]),
                        Duration = Convert.ToInt32(reader["Duration"]),
                        DueDate = Convert.ToDateTime(reader["DueDate"]),
                        Task1 = reader["Task1"].ToString(),
                        Task2 = reader["Task2"].ToString(),
                        Task3 = reader["Task3"].ToString(),
                        Task4 = reader["Task4"].ToString(),
                        Task5 = reader["Task5"].ToString()
                    };

                    activities.Add(activity);
                }
            }
        }

        return activities;
    }
}


using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

public class ActivitiesController : Controller
{
    private readonly ActivityService _activityService;

    public ActivitiesController(IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection");
        _activityService = new ActivityService(connectionString);
    }

    public IActionResult Index()
    {
        List<Activity> activities = _activityService.GetAllActivities();
        return View(activities);
    }
}



@model IEnumerable<Activity>

<h2>Activities</h2>

<table class="table table-bordered">
    <thead>
        <tr>
            <th>ID</th>
            <th>Description</th>
            <th>Client</th>
            <th>Start Date</th>
            <th>Duration (hours)</th>
            <th>Due Date</th>
            <th>Task 1</th>
            <th>Task 2</th>
            <th>Task 3</th>
            <th>Task 4</th>
            <th>Task 5</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var activity in Model)
        {
            <tr>
                <td>@activity.ID</td>
                <td>@activity.Description</td>
                <td>@activity.Client</td>
                <td>@activity.StartDate.ToString("g")</td>
                <td>@activity.Duration</td>
                <td>@activity.DueDate.ToString("g")</td>
                <td>@activity.Task1</td>
                <td>@activity.Task2</td>
                <td>@activity.Task3</td>
                <td>@activity.Task4</td>
                <td>@activity.Task5</td>
            </tr>
        }
    </tbody>
</table>



<nav class="navbar navbar-expand-lg navbar-light bg-light">
    <a class="navbar-brand" href="#">Activity Manager</a>
    <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
        <span class="navbar-toggler-icon"></span>
    </button>
    <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav">
            <li class="nav-item">
                <a class="nav-link" asp-controller="Home" asp-action="Index">Home</a>
            </li>
            <li class="nav-item">
                <a class="nav-link" asp-controller="Activities" asp-action="Index">Activities</a>
            </li>
        </ul>
    </div>
</nav>
