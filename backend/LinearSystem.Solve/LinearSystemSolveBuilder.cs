using LinearSystem.Solve.Methods;
using LinearSystem.Solve.Methods.GaussSeidel;
using LinearSystem.Solve.Methods.Jacobi;
using LinearSystem.Solve.Methods.Matrix;
using LinearSystem.Solve.Methods.Relaxation;
using LinearSystem.Solve.Tools;

namespace LinearSystem.Solve;

public static class LinearSystemSolveBuilder
{
    public static IIterativeSolveMethod CreateIterative(SolveMethods methodType)
    {
        return methodType switch
        {
            SolveMethods.Jacobi => new JacobiMethod(),
            SolveMethods.GaussSeidel => new GaussSeidelMethod(),
            SolveMethods.Relaxation => new RelaxationMethod()
        };
    }

    public static Task<TestResult> RunTest(List<(SquareMatrix a, VectorColumn b, double error, VectorColumn? initial)> systems, CancellationToken token)
    {
        return Task.Run(() =>
        {
            List<LinearSystemSolveResult> solveSystemsResults = new List<LinearSystemSolveResult>();
            foreach (var (a, b, error, initial) in systems)
            {
                List<LinearSystemSolveMethodResult> solveSystemResults = new List<LinearSystemSolveMethodResult>();
                VectorColumn explicitRoots = new MatrixMethod().Calculate(a, b);
                List<IIterativeSolveMethod> methods = new List<IIterativeSolveMethod>()
                {
                    new JacobiMethod(), 
                    new GaussSeidelMethod(), 
                    new RelaxationMethod()
                };
                foreach (IIterativeSolveMethod method in methods)
                {
                    LinearSystemSolveWithStepMethodResult result = method.Calculate(a, b, error, token, initial);
                    VectorColumn roots = result.Answers.Last().Roots;
                    double absoluteError = (explicitRoots - roots).GetEuclideanNorm();
                    double relativeError = absoluteError / explicitRoots.GetEuclideanNorm();
                    solveSystemResults.Add(new LinearSystemSolveMethodResult(roots, result.IterationCount, result.ExecutingTime, method.MethodName, absoluteError, relativeError));
                }
                solveSystemsResults.Add( new LinearSystemSolveResult(solveSystemResults, explicitRoots));
            }

            return new TestResult(solveSystemsResults);
        },token);
    }
}