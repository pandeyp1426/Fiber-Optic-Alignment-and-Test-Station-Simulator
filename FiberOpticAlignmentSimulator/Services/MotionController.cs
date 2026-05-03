using FiberOpticAlignmentSimulator.Models;

namespace FiberOpticAlignmentSimulator.Services;

/// <summary>
/// Simulates a two-axis motion controller and performs a simple peak-search alignment routine.
/// </summary>
public class MotionController
{
    /// <summary>
    /// Moves the part by a relative X/Y amount.
    /// </summary>
    public void MoveRelative(FiberPart part, double xMove, double yMove)
    {
        part.XOffset += xMove;
        part.YOffset += yMove;
    }

    /// <summary>
    /// Searches around the current position and steps toward the position with the highest measured power.
    /// </summary>
    public void AlignToPeak(FiberPart part, PowerSensor powerSensor)
    {
        double stepSize = 0.25;
        const double minimumStepSize = 0.01;
        const int maxIterations = 60;

        part.PowerMW = powerSensor.MeasurePower(part.XOffset, part.YOffset);

        for (int i = 0; i < maxIterations && stepSize >= minimumStepSize; i++)
        {
            double bestXMove = 0.0;
            double bestYMove = 0.0;
            double bestPower = part.PowerMW;

            CheckCandidate(part, powerSensor, stepSize, 0.0, ref bestXMove, ref bestYMove, ref bestPower);
            CheckCandidate(part, powerSensor, -stepSize, 0.0, ref bestXMove, ref bestYMove, ref bestPower);
            CheckCandidate(part, powerSensor, 0.0, stepSize, ref bestXMove, ref bestYMove, ref bestPower);
            CheckCandidate(part, powerSensor, 0.0, -stepSize, ref bestXMove, ref bestYMove, ref bestPower);

            if (bestXMove == 0.0 && bestYMove == 0.0)
            {
                stepSize *= 0.5;
                continue;
            }

            MoveRelative(part, bestXMove, bestYMove);
            part.PowerMW = bestPower;
        }

        part.PowerMW = powerSensor.MeasurePower(part.XOffset, part.YOffset);
    }

    private static void CheckCandidate(
        FiberPart part,
        PowerSensor powerSensor,
        double xMove,
        double yMove,
        ref double bestXMove,
        ref double bestYMove,
        ref double bestPower)
    {
        double candidatePower = powerSensor.MeasurePower(part.XOffset + xMove, part.YOffset + yMove);

        if (candidatePower > bestPower)
        {
            bestXMove = xMove;
            bestYMove = yMove;
            bestPower = candidatePower;
        }
    }
}
