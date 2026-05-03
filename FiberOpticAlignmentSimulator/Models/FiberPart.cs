namespace FiberOpticAlignmentSimulator.Models;

/// <summary>
/// Represents one fiber optic or laser module currently loaded in the virtual station.
/// </summary>
public class FiberPart
{
    /// <summary>
    /// Creates a part with a serial number and initial alignment offsets.
    /// </summary>
    public FiberPart(string serialNumber, double xOffset, double yOffset)
    {
        SerialNumber = serialNumber;
        XOffset = xOffset;
        YOffset = yOffset;
    }

    public string SerialNumber { get; }

    public double XOffset { get; set; }

    public double YOffset { get; set; }

    public double PowerMW { get; set; }

    public double TemperatureC { get; set; }
}
