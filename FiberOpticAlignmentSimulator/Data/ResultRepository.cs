using FiberOpticAlignmentSimulator.Models;
using Microsoft.Data.Sqlite;

namespace FiberOpticAlignmentSimulator.Data;

/// <summary>
/// Handles SQLite database creation and test result storage.
/// </summary>
public class ResultRepository
{
    private readonly string _connectionString;

    public ResultRepository(string databasePath)
    {
        string? directory = Path.GetDirectoryName(databasePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = databasePath
        }.ToString();
    }

    /// <summary>
    /// Creates the database table if it does not already exist.
    /// </summary>
    public void InitializeDatabase()
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS TestRuns
            (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                SerialNumber TEXT NOT NULL,
                StartTime TEXT NOT NULL,
                EndTime TEXT NOT NULL,
                XOffset REAL NOT NULL,
                YOffset REAL NOT NULL,
                PowerMW REAL NOT NULL,
                TemperatureC REAL NOT NULL,
                Passed INTEGER NOT NULL,
                FailureReason TEXT NOT NULL
            );
            """;

        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Inserts one completed test run into SQLite.
    /// </summary>
    public void Save(TestResult result)
    {
        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO TestRuns
            (
                SerialNumber,
                StartTime,
                EndTime,
                XOffset,
                YOffset,
                PowerMW,
                TemperatureC,
                Passed,
                FailureReason
            )
            VALUES
            (
                $serialNumber,
                $startTime,
                $endTime,
                $xOffset,
                $yOffset,
                $powerMW,
                $temperatureC,
                $passed,
                $failureReason
            );
            """;

        command.Parameters.AddWithValue("$serialNumber", result.SerialNumber);
        command.Parameters.AddWithValue("$startTime", result.StartTime.ToString("O"));
        command.Parameters.AddWithValue("$endTime", result.EndTime.ToString("O"));
        command.Parameters.AddWithValue("$xOffset", result.XOffset);
        command.Parameters.AddWithValue("$yOffset", result.YOffset);
        command.Parameters.AddWithValue("$powerMW", result.PowerMW);
        command.Parameters.AddWithValue("$temperatureC", result.TemperatureC);
        command.Parameters.AddWithValue("$passed", result.Passed ? 1 : 0);
        command.Parameters.AddWithValue("$failureReason", result.FailureReason);

        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Returns saved test runs with the newest records first.
    /// </summary>
    public List<TestResult> GetAll()
    {
        List<TestResult> results = new();

        using SqliteConnection connection = new(_connectionString);
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT
                Id,
                SerialNumber,
                StartTime,
                EndTime,
                XOffset,
                YOffset,
                PowerMW,
                TemperatureC,
                Passed,
                FailureReason
            FROM TestRuns
            ORDER BY Id DESC;
            """;

        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            results.Add(new TestResult
            {
                Id = reader.GetInt32(0),
                SerialNumber = reader.GetString(1),
                StartTime = DateTime.Parse(reader.GetString(2)),
                EndTime = DateTime.Parse(reader.GetString(3)),
                XOffset = reader.GetDouble(4),
                YOffset = reader.GetDouble(5),
                PowerMW = reader.GetDouble(6),
                TemperatureC = reader.GetDouble(7),
                Passed = reader.GetInt32(8) == 1,
                FailureReason = reader.GetString(9)
            });
        }

        return results;
    }
}
