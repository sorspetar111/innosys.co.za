 using System;

public class BusinessTimeCalculator
{
    private readonly TimeSpan BusinessStartTime = new TimeSpan(8, 30, 0); // 08:30
    private readonly TimeSpan BusinessEndTime = new TimeSpan(16, 0, 0);   // 16:00
    private readonly TimeSpan BusinessDayDuration = new TimeSpan(7, 30, 0); // 7.5 hours (08:30 to 16:00)
    
    public DateTime CalculateDueDate(DateTime startDate, int duration)
    {
        DateTime dueDate = startDate;
        TimeSpan remainingTime = TimeSpan.FromHours(duration);

        while (remainingTime > TimeSpan.Zero)
        {
            // Check if current time is within business hours
            if (IsBusinessDay(dueDate) && IsWithinBusinessHours(dueDate))
            {
                // Calculate how much time we have left in the current business day
                TimeSpan timeLeftInDay = BusinessEndTime - dueDate.TimeOfDay;

                if (timeLeftInDay >= remainingTime)
                {
                    // If the remaining time fits within the current business hours
                    dueDate = dueDate.Add(remainingTime);
                    remainingTime = TimeSpan.Zero;
                }
                else
                {
                    // Otherwise, use up the rest of the business hours and move to the next business day
                    dueDate = dueDate.Add(timeLeftInDay);
                    remainingTime -= timeLeftInDay;
                    dueDate = MoveToNextBusinessDay(dueDate);
                    dueDate = dueDate.Date + BusinessStartTime; // Start at 08:30 the next business day
                }
            }
            else
            {
                // If not within business hours, move to the next business day
                dueDate = MoveToNextBusinessDay(dueDate);
                dueDate = dueDate.Date + BusinessStartTime; // Start at 08:30 the next business day
            }
        }

        return dueDate;
    }

    private bool IsBusinessDay(DateTime date)
    {
        // Monday = 1, ..., Friday = 5
        return date.DayOfWeek >= DayOfWeek.Monday && date.DayOfWeek <= DayOfWeek.Friday;
    }

    private bool IsWithinBusinessHours(DateTime date)
    {
        return date.TimeOfDay >= BusinessStartTime && date.TimeOfDay < BusinessEndTime;
    }

    private DateTime MoveToNextBusinessDay(DateTime date)
    {
        do
        {
            date = date.AddDays(1);
        } while (!IsBusinessDay(date));

        return date;
    }
}


public class DatabaseHandler
{
    private readonly string _connectionString;
    private readonly BusinessTimeCalculator _calculator;

    public DatabaseHandler(string connectionString)
    {
        _connectionString = connectionString;
        _calculator = new BusinessTimeCalculator();
    }

    public void InsertActivities(List<Activity> activities)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            foreach (var activity in activities)
            {
                DateTime dueDate = _calculator.CalculateDueDate(activity.StartDate, activity.Duration);

                using (var command = new SqlCommand("INSERT INTO Activities (ID, Description, Client, StartDate, Duration, DueDate, Task1, Task2, Task3, Task4, Task5) VALUES (@ID, @Description, @Client, @StartDate, @Duration, @DueDate, @Task1, @Task2, @Task3, @Task4, @Task5)", connection))
                {
                    command.Parameters.AddWithValue("@ID", activity.ID);
                    command.Parameters.AddWithValue("@Description", activity.Description);
                    command.Parameters.AddWithValue("@Client", activity.Client);
                    command.Parameters.AddWithValue("@StartDate", activity.StartDate);
                    command.Parameters.AddWithValue("@Duration", activity.Duration);
                    command.Parameters.AddWithValue("@DueDate", dueDate); // Insert the calculated due date
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

        Console.WriteLine("CSV data has been successfully inserted into the database with due dates calculated.");
    }
}


