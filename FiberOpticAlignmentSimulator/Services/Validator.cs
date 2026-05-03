using FiberOpticAlignmentSimulator.Models;

namespace FiberOpticAlignmentSimulator.Services;

/// <summary>
/// Applies station tolerance checks and returns a clear pass/fail result.
/// </summary>
public class Validator
{
    /// <summary>
    /// Validates final alignment, optical power, and temperature.
    /// </summary>
    public ValidationResult Validate(FiberPart part)
    {
        List<string> failures = new();

        if (Math.Abs(part.XOffset) > StationSettings.AlignmentToleranceMm ||
            Math.Abs(part.YOffset) > StationSettings.AlignmentToleranceMm)
        {
            failures.Add($"Alignment outside +/-{StationSettings.AlignmentToleranceMm:F2} mm tolerance");
        }

        if (part.PowerMW < StationSettings.MinimumPowerMW)
        {
            failures.Add($"Power below {StationSettings.MinimumPowerMW:F2} mW minimum");
        }

        if (part.TemperatureC < StationSettings.MinimumTemperatureC ||
            part.TemperatureC > StationSettings.MaximumTemperatureC)
        {
            failures.Add($"Temperature outside {StationSettings.MinimumTemperatureC:F1}-{StationSettings.MaximumTemperatureC:F1} C range");
        }

        return failures.Count == 0
            ? new ValidationResult(true, string.Empty)
            : new ValidationResult(false, string.Join("; ", failures));
    }
}
