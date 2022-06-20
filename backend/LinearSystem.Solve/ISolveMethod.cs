using LinearSystem.Solve.Tools;

namespace LinearSystem.Solve;
public interface ISolveMethod
{
    VectorColumn Calculate(SquareMatrix a, VectorColumn b, VectorColumn initialGuess);
}