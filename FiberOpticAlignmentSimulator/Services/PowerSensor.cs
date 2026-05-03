namespace FiberOpticAlignmentSimulator.Services;

/// <summary>
/// Simulates an optical power meter reading from the current X/Y alignment position.
/// </summary>
public class PowerSensor
{
    private readonly Random _random = new();

    /// <summary>
    /// Calculates optical power. Power increases as both offsets approach zero.
    /// </summary>
    public double MeasurePower(double xOffset, double yOffset, bool includeNoise = true)
    {
        double distanceSquared = xOffset * xOffset + yOffset * yOffset;
        double sigmaSquared = StationSettings.AlignmentSigmaMm * StationSettings.AlignmentSigmaMm;
        double coupledPower = StationSettings.MaxSimulatedPowerMW * Math.Exp(-distanceSquared / (2.0 * sigmaSquared));
        double noise = includeNoise ? (_random.NextDouble() - 0.5) * 0.04 : 0.0;

        return Math.Max(0.0, coupledPower + noise);
    }
}
