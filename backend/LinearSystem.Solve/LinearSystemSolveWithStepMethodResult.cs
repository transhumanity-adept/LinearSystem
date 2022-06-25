using LinearSystem.Solve.Tools;

namespace LinearSystem.Solve;

public struct LinearSystemSolveWithStepMethodResult
{
    public LinearSystemSolveWithStepMethodResult(List<SolveStep> answers, int iterationCount, TimeSpan executingTime)
    {
        Answers = answers;
        IterationCount = iterationCount;
        ExecutingTime = executingTime;
    }
    public List<SolveStep> Answers { get; }
    public int IterationCount { get; }
    public TimeSpan ExecutingTime { get; }
}