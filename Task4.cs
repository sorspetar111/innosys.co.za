



using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class SqlScriptGenerator
{
    public string GenerateSqlScript(List<Activity> activities)
    {
        StringBuilder scriptBuilder = new StringBuilder();

        // Step 1: Add table creation script
        scriptBuilder.AppendLine("CREATE TABLE Activities (");
        scriptBuilder.AppendLine("    ID INT PRIMARY KEY,");
        scriptBuilder.AppendLine("    Description NVARCHAR(255) NOT NULL,");
        scriptBuilder.AppendLine("    Client NVARCHAR(255) NOT NULL,");
        scriptBuilder.AppendLine("    StartDate DATETIME NOT NULL,");
        scriptBuilder.AppendLine("    Duration INT NOT NULL,");
        scriptBuilder.AppendLine("    DueDate DATETIME NOT NULL,");
        scriptBuilder.AppendLine("    Task1 NVARCHAR(255),");
        scriptBuilder.AppendLine("    Task2 NVARCHAR(255),");
        scriptBuilder.AppendLine("    Task3 NVARCHAR(255),");
        scriptBuilder.AppendLine("    Task4 NVARCHAR(255),");
        scriptBuilder.AppendLine("    Task5 NVARCHAR(255)");
        scriptBuilder.AppendLine(");");
        scriptBuilder.AppendLine();

        // Step 2: Add data insertion script for each activity
        foreach (var activity in activities)
        {
            scriptBuilder.AppendLine("INSERT INTO Activities (ID, Description, Client, StartDate, Duration, DueDate, Task1, Task2, Task3, Task4, Task5)");
            scriptBuilder.AppendFormat("VALUES ({0}, '{1}', '{2}', '{3}', {4}, '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');",
                activity.ID,
                SanitizeSqlString(activity.Description),
                SanitizeSqlString(activity.Client),
                activity.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                activity.Duration,
                activity.DueDate.ToString("yyyy-MM-dd HH:mm:ss"),
                SanitizeSqlString(activity.Task1),
                SanitizeSqlString(activity.Task2),
                SanitizeSqlString(activity.Task3),
                SanitizeSqlString(activity.Task4),
                SanitizeSqlString(activity.Task5)
            );
            scriptBuilder.AppendLine();
        }

        return scriptBuilder.ToString();
    }

    private string SanitizeSqlString(string input)
    {
        return input?.Replace("'", "''") ?? "NULL";
    }
}



/* SQL Managemnt Studio */
IF OBJECT_ID('dbo.Activities', 'U') IS NOT NULL
    DROP TABLE dbo.Activities;

CREATE TABLE Activities (
    ID INT PRIMARY KEY,
    Description NVARCHAR(255) NOT NULL,
    Client NVARCHAR(255) NOT NULL,
    StartDate DATETIME NOT NULL,
    Duration INT NOT NULL,
    DueDate DATETIME NOT NULL,
    Task1 NVARCHAR(255),
    Task2 NVARCHAR(255),
    Task3 NVARCHAR(255),
    Task4 NVARCHAR(255),
    Task5 NVARCHAR(255)
);

-- Inserts
INSERT INTO Activities (ID, Description, Client, StartDate, Duration, Task1, Task2, Task3, Task4, Task5)
VALUES
(1, 'Check Application', 'Mr Ndlovu', '2021-04-26T09:00:00', 3, 'Check all fields completed', 'Cross validate fields', 'Check within limits', 'Issue Cheque', 'Post Cheque'),
(2, 'Check Application', 'Mrs Smith', '2021-04-27T08:30:00', 4, 'Check all fields completed', 'Cross validate fields', 'Check within limits', 'Issue Cheque', 'Post Cheque'),
(3, 'Approve Limit Increase', 'Mr van der Spuy', '2021-04-28T15:30:00', 4, 'Check Current facility', 'Check collateral', '', '', ''),
(4, 'Check Application', 'Mr Baros', '2021-04-29T13:30:00', 8, 'Check all fields completed', 'Cross validate fields', 'Check within limits', 'Issue Cheque', 'Post Cheque'),
(5, 'Approve Limit Increase', 'Ms Mokoena', '2021-04-30T12:25:00', 18, 'Check Current facility', 'Check collateral', '', '', '');

