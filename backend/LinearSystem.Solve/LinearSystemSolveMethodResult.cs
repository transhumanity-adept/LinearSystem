using LinearSystem.Solve.Tools;

namespace LinearSystem.Solve;

public struct LinearSystemSolveMethodResult
{
    public LinearSystemSolveMethodResult(VectorColumn roots, int iterationCount, TimeSpan executingTime, string methodName, double absoluteError, double relativeError)
    {
        IterationCount = iterationCount;
        ExecutingTime = executingTime;
        RelativeError = relativeError;
        AbsoluteError = absoluteError;
        MethodName = methodName; 
        Roots = roots;
    }
    public VectorColumn Roots { get; }
    public int IterationCount { get; }
    public TimeSpan ExecutingTime { get; }
    public string MethodName { get; }
    public double AbsoluteError { get; }
    public double RelativeError { get; }
}