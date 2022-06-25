using LinearSystem.Solve.Tools;

namespace LinearSystem.Solve;
public interface IIterativeSolveMethod
{
    string MethodName { get; }
    LinearSystemSolveWithStepMethodResult Calculate(SquareMatrix a, VectorColumn b, double error, CancellationToken token, VectorColumn initialGuess = null);
}