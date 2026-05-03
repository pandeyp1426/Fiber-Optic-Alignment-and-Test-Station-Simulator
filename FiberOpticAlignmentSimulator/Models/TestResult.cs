namespace FiberOpticAlignmentSimulator.Models;

/// <summary>
/// Stores the final measured values and validation result for one completed station run.
/// </summary>
public class TestResult
{
    public int Id { get; set; }

    public string SerialNumber { get; set; } = string.Empty;

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public double XOffset { get; set; }

    public double YOffset { get; set; }

    public double PowerMW { get; set; }

    public double TemperatureC { get; set; }

    public bool Passed { get; set; }

    public string FailureReason { get; set; } = string.Empty;
}
