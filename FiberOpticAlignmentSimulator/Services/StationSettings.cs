namespace FiberOpticAlignmentSimulator.Services;

/// <summary>
/// Central location for station tolerances and simulation constants.
/// </summary>
public static class StationSettings
{
    public const double AlignmentToleranceMm = 0.08;
    public const double MinimumPowerMW = 1.80;
    public const double MinimumTemperatureC = 18.0;
    public const double MaximumTemperatureC = 30.0;
    public const double MaxSimulatedPowerMW = 2.50;
    public const double AlignmentSigmaMm = 0.35;
}
