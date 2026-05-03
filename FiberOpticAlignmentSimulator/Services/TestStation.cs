using FiberOpticAlignmentSimulator.Models;

namespace FiberOpticAlignmentSimulator.Services;

/// <summary>
/// Coordinates loading a part, running alignment, measuring values, and building the final result.
/// </summary>
public class TestStation
{
    private readonly MotionController _motionController = new();
    private readonly PowerSensor _powerSensor = new();
    private readonly Validator _validator = new();
    private readonly Random _random = new();

    /// <summary>
    /// Creates a new simulated part with a serial number and starting alignment error.
    /// </summary>
    public FiberPart GeneratePart(string? serialNumber = null)
    {
        string generatedSerial = string.IsNullOrWhiteSpace(serialNumber)
            ? $"FO-{DateTime.Now:yyyyMMdd-HHmmss}"
            : serialNumber.Trim();

        double xOffset = RandomOffset();
        double yOffset = RandomOffset();

        return new FiberPart(generatedSerial, xOffset, yOffset)
        {
            PowerMW = _powerSensor.MeasurePower(xOffset, yOffset),
            TemperatureC = SimulateTemperature()
        };
    }

    /// <summary>
    /// Runs the full virtual station sequence for the provided part.
    /// </summary>
    public TestResult RunTest(FiberPart part)
    {
        DateTime startTime = DateTime.Now;

        _motionController.AlignToPeak(part, _powerSensor);
        part.TemperatureC = SimulateTemperature();

        ValidationResult validation = _validator.Validate(part);

        return new TestResult
        {
            SerialNumber = part.SerialNumber,
            StartTime = startTime,
            EndTime = DateTime.Now,
            XOffset = part.XOffset,
            YOffset = part.YOffset,
            PowerMW = part.PowerMW,
            TemperatureC = part.TemperatureC,
            Passed = validation.Passed,
            FailureReason = validation.FailureReason
        };
    }

    private double RandomOffset()
    {
        return (_random.NextDouble() * 1.6) - 0.8;
    }

    private double SimulateTemperature()
    {
        double normalReading = 23.0 + (_random.NextDouble() * 4.5);

        if (_random.NextDouble() < 0.08)
        {
            return 31.0 + (_random.NextDouble() * 2.0);
        }

        return normalReading;
    }
}
