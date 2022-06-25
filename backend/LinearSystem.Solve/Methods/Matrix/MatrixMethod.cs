using LinearSystem.Solve.Exceptions;
using LinearSystem.Solve.Tools;

namespace LinearSystem.Solve.Methods.Matrix;

public class MatrixMethod: IExplicitSolveMethod
{

    public VectorColumn Calculate(SquareMatrix a, VectorColumn b)
    {
        SolveException.ThrowIf(a.GetDeterminant() == 0, "Determinant must be nonzero");
        return a.GetInvertedMatrix() * b;
    }
}