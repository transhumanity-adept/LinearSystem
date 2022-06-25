using LinearSystem.Solve.Tools;

namespace LinearSystem.Solve;

public struct LinearSystemSolveResult
{
    public LinearSystemSolveResult(List<LinearSystemSolveMethodResult> methodsResults, VectorColumn explicitRoots)
    {
        MethodsResults = methodsResults;
        ExplicitRoots = explicitRoots;
    }

    public List<LinearSystemSolveMethodResult> MethodsResults { get; }
    public VectorColumn ExplicitRoots { get; }
}