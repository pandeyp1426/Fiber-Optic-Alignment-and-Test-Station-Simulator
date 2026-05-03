namespace FiberOpticAlignmentSimulator.Models;

/// <summary>
/// Contains the pass/fail decision and readable failure reason from station validation.
/// </summary>
public class ValidationResult
{
    public ValidationResult(bool passed, string failureReason)
    {
        Passed = passed;
        FailureReason = failureReason;
    }

    public bool Passed { get; }

    public string FailureReason { get; }
}
