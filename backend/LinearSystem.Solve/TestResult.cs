namespace LinearSystem.Solve;

public struct TestResult
{
    public List<LinearSystemSolveResult> Results { get; }

    public TestResult(List<LinearSystemSolveResult> results)
    {
        Results = results;
    }
}