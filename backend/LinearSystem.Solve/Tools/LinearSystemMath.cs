using System.Numerics;

namespace LinearSystem.Solve.Tools;

public static class LinearSystemMath
{
    private static Complex Acosh(double x)
    {
        if (x < 1) throw new Exception("Аргумент функции не входит в область определения");
        return Complex.Log(x + Complex.Sqrt(x * x - 1));
    }

    private static Complex Asinh(double x)
    {
        return Complex.Log(x + Complex.Sqrt(x * x + 1));
    }

    public static List<Complex> GetRootsLinearFunction(double k, double b)
    {
        if (k == 0) throw new Exception("Коэффициент k равен нулю");
        return new List<Complex>() {-b / k};
    }

    public static List<Complex> GetRootsQuadraticFunction(double a, double b, double c)
    {
        double discriminant = Math.Pow(b, 2) - 4 * a * c;

        return new List<Complex>()
        {
            (-b + Complex.Sqrt(discriminant)) / (2 * a),
            (-b - Complex.Sqrt(discriminant)) / (2 * a)
        };
    }

    public static List<Complex> GetRootsCubicFunction(double a, double b, double c, double d)
    {
        double a1 = b / a;
        double b1 = c / a;
        double c1 = d / a;

        double q = (Math.Pow(a1, 2) - 3 * b1) / 9;
        double r = (2 * Math.Pow(a1, 3) - 9 * a1 * b1 + 27 * c1) / 54;

        double s = Math.Pow(q, 3) - Math.Pow(r, 2);

        if (s > 0)
        {
            double phi = 1.0 / 3 * Math.Acos(r / Math.Sqrt(Math.Pow(q, 3)));
            return new List<Complex>()
            {
                -2 * Complex.Sqrt(q) * Complex.Cos(phi) - a1 / 3,
                -2 * Complex.Sqrt(q) * Complex.Cos(phi + 2.0 / 3 * Math.PI) - a1 / 3,
                -2 * Complex.Sqrt(q) * Complex.Cos(phi - 2.0 / 3 * Math.PI) - a1 / 3
            };
        }
        else if (s < 0)
        {
            if (q > 0)
            {
                Complex phi = 1.0 / 3 * Acosh(Math.Abs(r) / Math.Sqrt(q * q * q));
                return new List<Complex>()
                {
                    -2 * Math.Sign(r) * Math.Sqrt(q) * Complex.Cosh(phi) - a1 / 3,
                    new Complex((Math.Sign(r) * Complex.Sqrt(q) * Complex.Cosh(phi)).Real - a1 / 3,
                        (Math.Sqrt(3) * Math.Sqrt(q) * Complex.Sinh(phi)).Real),
                    new Complex((Math.Sign(r) * Complex.Sqrt(q) * Complex.Cosh(phi)).Real - a1 / 3,
                        (-Math.Sqrt(3) * Math.Sqrt(q) * Complex.Sinh(phi)).Real)
                };
            }
            else
            {
                Complex phi = 1.0 / 3 * Asinh(Math.Abs(r) / Math.Sqrt(Math.Pow(Math.Abs(q), 3)));
                return new List<Complex>()
                {
                    -2 * Math.Sign(r) * Complex.Sqrt(Math.Abs(q)) * Complex.Sinh(phi) - a1 / 3,
                    new Complex((Math.Sign(r) * Complex.Sqrt(Math.Abs(q)) * Complex.Sinh(phi) - a1 / 3).Real,
                        (Math.Sqrt(3) * Complex.Sqrt(Math.Abs(q)) * Complex.Cosh(phi)).Real),
                    new Complex((Math.Sign(r) * Complex.Sqrt(Math.Abs(q)) * Complex.Sinh(phi) - a1 / 3).Real,
                        (-Math.Sqrt(3) * Complex.Sqrt(Math.Abs(q)) * Complex.Cosh(phi)).Real)
                };
            }
        }
        else
        {
            return new List<Complex>()
            {
                -2 * Math.Sign(r) * Complex.Sqrt(q) - a1 / 3,
                Math.Sign(r) * Complex.Sqrt(q) - a1 / 3,
                Math.Sign(r) * Complex.Sqrt(q) - a1 / 3
            };
        }
    }

    public static List<Complex> GetRootsFunctionFourthDegree(double a, double b, double c, double d, double e)
    {
        double a3 = b / a;
        double a2 = c / a;
        double a1 = d / a;
        double a0 = e / a;

        List<Complex> tmp_roots =
            GetRootsCubicFunction(1, -a2, a1 * a3 - 4 * a0, -(a1 * a1 + a0 * a3 * a3 - 4 * a0 * a2));
        double selected_real_root = 0;
        if (tmp_roots.TrueForAll(value => value.Imaginary == 0))
        {
            foreach (var root in tmp_roots)
            {
                if (a3 * a3 / 4 + root.Real - a2 >= 0)
                {
                    selected_real_root = root.Real;
                    break;
                }
            }
        }
        else
        {
            selected_real_root = tmp_roots.Find(value => value.Imaginary == 0).Real;
        }

        double p1 = a3 / 2 + Math.Sqrt(a3 * a3 / 4 + selected_real_root - a2);
        double p2 = a3 / 2 - Math.Sqrt(a3 * a3 / 4 + selected_real_root - a2);
        double q1 = selected_real_root / 2 + Math.Sqrt(selected_real_root * selected_real_root / 4 - a0);
        double q2 = selected_real_root / 2 - Math.Sqrt(selected_real_root * selected_real_root / 4 - a0);
        if (Math.Round(p1 * q2 + p2 * q1, 2) == Math.Round(a1, 2))
        {
            List<Complex> r1 = GetRootsQuadraticFunction(1, p1, q1);
            List<Complex> r2 = GetRootsQuadraticFunction(1, p2, q2);
            return new List<Complex>()
            {
                r1[0],
                r1[1],
                r2[0],
                r2[1]
            };
        }
        else
        {
            List<Complex> r1 = GetRootsQuadraticFunction(1, p1, q2);
            List<Complex> r2 = GetRootsQuadraticFunction(1, p2, q1);
            return new List<Complex>()
            {
                r1[0],
                r1[1],
                r2[0],
                r2[1]
            };
        }
    }

    public static List<Complex> SolveMatrixWithLambda(LambdaExpression[,] matrixWithLambda)
    {
        List<double> coefficients = GetLambdaExpressionRecursive(matrixWithLambda)
            .LambdaDigits
            .OrderByDescending(digit => digit.LambdaDegree)
            .Select(digit => digit.Coefficient)
            .ToList();

        return coefficients.Count switch
        {
            2 => LinearSystemMath.GetRootsLinearFunction(coefficients[0], coefficients[1]),
            3 => LinearSystemMath.GetRootsQuadraticFunction(coefficients[0], coefficients[1], coefficients[2]),
            4 => LinearSystemMath.GetRootsCubicFunction(coefficients[0], coefficients[1], coefficients[2],
                coefficients[3]),
            5 => LinearSystemMath.GetRootsFunctionFourthDegree(coefficients[0], coefficients[1], coefficients[2],
                coefficients[3],
                coefficients[4]),
            _ => new List<Complex>()
        };
    }

    public static LambdaExpression GetLambdaExpressionRecursive(LambdaExpression[,] matrix)
    {
        if (matrix.GetLength(0) == 2)
        {
            return matrix[0, 0] * matrix[1, 1] - matrix[1, 0] * matrix[0, 1] +
                   new LambdaExpression(new LambdaDigit(0, 0));
        }

        LambdaExpression result = new LambdaExpression(new LambdaDigit(0, 0));
        for (int j = 0; j < matrix.GetLength(1); j++)
        {
            result += (j % 2 == 0 ? matrix[0, j] : -matrix[0, j]) *
                      GetLambdaExpressionRecursive(GetLambdaMatrixWithoutRowAndColumn(matrix, 0, j));
        }

        return result;
    }

    public static LambdaExpression[,] GetLambdaMatrixWithoutRowAndColumn(LambdaExpression[,] matrix, int removalRow,
        int removalColumn)
    {
        List<List<LambdaExpression>> tmpMatrix = new List<List<LambdaExpression>>();
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            tmpMatrix.Add(new List<LambdaExpression>());
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                tmpMatrix[i].Add(matrix[i, j]);
            }
        }

        tmpMatrix.RemoveAt(removalRow);
        foreach (List<LambdaExpression> row in tmpMatrix)
        {
            row.RemoveAt(removalColumn);
        }

        LambdaExpression[,] result = new LambdaExpression[matrix.GetLength(0) - 1, matrix.GetLength(1) - 1];
        for (int i = 0; i < result.GetLength(0); i++)
        {
            for (int j = 0; j < result.GetLength(1); j++)
            {
                result[i, j] = tmpMatrix[i][j];
            }
        }

        return result;
    }
}