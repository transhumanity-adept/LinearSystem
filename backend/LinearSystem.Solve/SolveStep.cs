using LinearSystem.Solve.Tools;

namespace LinearSystem.Solve;

public struct SolveStep
{
    public VectorColumn Roots { get; }
    public double? Difference { get; }

    public SolveStep(VectorColumn roots, double? difference)
    {
        Roots = roots;
        Difference = difference;
    }
}