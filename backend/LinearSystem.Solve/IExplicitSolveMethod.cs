using LinearSystem.Solve.Tools;

namespace LinearSystem.Solve;

public interface IExplicitSolveMethod
{
    VectorColumn Calculate(SquareMatrix a, VectorColumn b);
}