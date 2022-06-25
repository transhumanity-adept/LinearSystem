﻿using System.Diagnostics;
using System.Numerics;
using LinearSystem.Solve.Exceptions;
using LinearSystem.Solve.Tools;

namespace LinearSystem.Solve.Methods.Relaxation;

public class RelaxationMethod: IIterativeSolveMethod
{
    private SquareMatrix _a;
    private VectorColumn _b;
    private VectorColumn _initial;
    private double _error;
    private CancellationToken _token;
    public string MethodName => "Relaxation";

    public LinearSystemSolveWithStepMethodResult Calculate(SquareMatrix a, VectorColumn b, double error, CancellationToken token,
        VectorColumn initialGuess = null)
    {
        _a = a;
        _b = b;
        _error = error;
        _initial = _initial ?? b;
        _token = token;
        Convergence convergence = CheckConvergence();
        SolveException.ThrowIf(convergence == Convergence.NotConvergence, "matrix is not convergence");
        return Solve();
    }
    
    private LinearSystemSolveWithStepMethodResult Solve()
    {
        VectorColumn lastRoots = _initial;
        List<(VectorColumn Roots, double? Difference)> answers = new List<(VectorColumn Roots, double? Difference)>()
        {
            (_initial, null)
        };
        Stopwatch timer = Stopwatch.StartNew();
        double currentDifference = double.MaxValue;
        double optimalRelaxationParameter = CalculateOptimalRelaxationParameter() ?? 1;
        do
        {
            if (_token.IsCancellationRequested) throw new Exception("Maximum execution time is exceeded");
            VectorColumn newRoots = new VectorColumn(lastRoots.Size);
            for (int i = 0; i < _a.Size; i++)
            {
                double sum = 0;

                for (int j = 0; j <= i - 1; j++)
                {
                    sum += _a[i, j] * newRoots[j];
                }
                for (int j = i + 1; j < _a.Size; j++)
                {
                    if (i == j) continue;
                    sum += _a[i, j] * lastRoots[j];
                }

                newRoots[i] = (1 - optimalRelaxationParameter)*lastRoots[i] + optimalRelaxationParameter / _a[i, i] * (_b[i] - sum);
            }
            currentDifference = (newRoots - lastRoots).GetMaximumNorm();
            lastRoots = new VectorColumn(newRoots);
            answers.Add((newRoots, currentDifference));
        } while (currentDifference > _error);
        timer.Stop();
        List<SolveStep> steps = answers.Select(answer => new SolveStep(answer.Roots, answer.Difference)).ToList();
        return new LinearSystemSolveWithStepMethodResult(steps, steps.Count, timer.Elapsed);
    }

    private double? CalculateOptimalRelaxationParameter()
    {
        var (d, l, u) = _a.GetDecomposition();
        SquareMatrix jacobi = d.GetInvertedMatrix() * (-l + -u);
        double? spectralRadius = jacobi.GetSpectralRadius();
        return spectralRadius is null ? spectralRadius : 2 / (1 + Math.Sqrt(1 - Math.Pow(spectralRadius.Value,2)));
    }
    
    private Convergence CheckConvergence()
    {
        if (_a.IsDiagonalPredominance()) return Convergence.Convergence;
        List<Complex> roots = LinearSystemMath.SolveMatrixWithLambda(GetNewRepresentationOfMatrix());
        if (roots.Count == 0) return Convergence.Unknown;
        return roots.All(root => Complex.Abs(root) < 1) ? Convergence.Convergence : Convergence.NotConvergence;
    }

    private LambdaExpression[,] GetNewRepresentationOfMatrix()
    {
        LambdaExpression[,] result = new LambdaExpression[_a.Size, _a.Size];
        for (int i = 0; i < _a.Size; i++)
        {
            for (int j = 0; j < _a.Size; j++)
            {
                result[i, j] = i >= j
                    ? new LambdaExpression(new LambdaDigit(_a[i, j], 1))
                    : new LambdaExpression(new LambdaDigit(_a[i, j], 0));
            }
        }

        return result;
    }
}