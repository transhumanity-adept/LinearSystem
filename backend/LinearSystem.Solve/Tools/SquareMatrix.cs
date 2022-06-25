using System.Numerics;
using System.Text;
using LinearSystem.Solve.Exceptions;

namespace LinearSystem.Solve.Tools;

public class SquareMatrix
{
    private List<List<double>> _data;
    public int Size => _data.Count;

    public SquareMatrix(int size)
    {
        SolveException.ThrowIf(
            condition: size < 1,
            message: $"param {nameof(size)} must be greater than 1");

        _data = new List<List<double>>(size);
    }

    public SquareMatrix(double[,] data)
    {
        SolveException.ThrowIf(
            condition: data is null,
            message: $"param {nameof(data)} must be not null");
        SolveException.ThrowIf(
            condition: data.GetLength(0) != data.GetLength(1),
            message: $"param {nameof(data)} must be square matrix");

        _data = new List<List<double>>();

        for (int i = 0; i < data.GetLength(0); i++)
        {
            _data.Add(new List<double>());
            for (int j = 0; j < data.GetLength(1); j++)
            {
                _data[i].Add(data[i, j]);
            }
        }
    }

    public SquareMatrix(IEnumerable<IEnumerable<double>> data)
    {
        _data = data.Select(row => row.ToList()).ToList();
    }

    public SquareMatrix(IEnumerable<VectorColumn> columns)
    {
        SolveException.ThrowIf(
            condition: columns is null,
            message: $"param {nameof(columns)} must be not null");
        SolveException.ThrowIf(
            condition: !columns.Any(),
            message: $"param {nameof(columns)} count must be greater than 1");
        int countRows = columns.ElementAt(0).Size;
        bool allColumnsAreTheSameSize = columns.All(column => column.Size == countRows);
        SolveException.ThrowIf(
            condition: !allColumnsAreTheSameSize,
            message: $"all columns must be the same size");
        int countColumns = columns.Count();
        SolveException.ThrowIf(
            condition: countRows != countColumns,
            message: $"{nameof(countRows)} must be equals {nameof(countColumns)}");

        _data = new List<List<double>>();
        for (int i = 0; i < countRows; i++)
        {
            _data.Add(new List<double>());
        }

        for (int j = 0; j < countColumns; j++)
        {
            for (int i = 0; i < countRows; i++)
            {
                _data[i].Add(columns.ElementAt(i)[j]);
            }
        }
    }

    public static SquareMatrix operator *(SquareMatrix matrix1, SquareMatrix matrix2)
    {
        double[,] data = new double[matrix1.Size, matrix2.Size];
        double sum = 0;
        for (int i = 0; i < matrix1.Size; i++)
        {
            for (int j = 0; j < matrix1.Size; j++)
            {
                for (int k = 0; k < matrix2.Size; k++)
                {
                    sum += matrix1[i, k] * matrix2[k, j];
                }

                data[i, j] = sum;
                sum = 0;
            }
        }

        return new SquareMatrix(data);
    }

    public static VectorColumn operator *(SquareMatrix matrix, VectorColumn vector)
    {
        return new VectorColumn(matrix._data.Zip(vector,
            (row, vectorItem) => row.Aggregate(0.0, (sum, rowValue) => sum + rowValue * vectorItem)));
    }

    public static SquareMatrix operator -(SquareMatrix matrix)
    {
        double[,] newData = new double[matrix.Size, matrix.Size];
        for (int i = 0; i < matrix.Size; i++)
        {
            for (int j = 0; j < matrix.Size; j++)
            {
                newData[i, j] = -matrix[i, j];
            }
        }

        return new SquareMatrix(newData);
    }

    public static SquareMatrix operator +(SquareMatrix matrix1, SquareMatrix matrix2)
    {
        double[,] newData = new double[matrix1.Size, matrix1.Size];
        for (int i = 0; i < matrix1.Size; i++)
        {
            for (int j = 0; j < matrix1.Size; j++)
            {
                newData[i, j] = matrix1[i, j] + matrix2[i, j];
            }
        }

        return new SquareMatrix(newData);
    }

    public double this[int rowIndex, int columnIndex]
    {
        get => _data[rowIndex][columnIndex];
        set => _data[rowIndex][columnIndex] = value;
    }

    public VectorColumn this[int columnIndex]
    {
        get
        {
            List<double> data = new List<double>();
            for (int i = 0; i < Size; i++)
            {
                data.Add(this[i, columnIndex]);
            }

            return new VectorColumn(data);
        }
    }

    public SquareMatrix GetTranspose()
    {
        double[,] data = new double[Size, Size];
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                data[i, j] = this[j, i];
            }
        }

        return new SquareMatrix(data);
    }

    public (SquareMatrix Diagonal, SquareMatrix LeftBottom, SquareMatrix RightTop) GetDecomposition()
    {
        double[,] matrixData = new double[Size, Size];
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                matrixData[i, j] = i == j ? this[i, j] : 0;
            }
        }

        SquareMatrix diagonal = new SquareMatrix(matrixData);

        matrixData = new double[Size, Size];
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                matrixData[i, j] = i > j ? this[i, j] : 0;
            }
        }

        SquareMatrix leftBottom = new SquareMatrix(matrixData);
        matrixData = new double[Size, Size];
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                matrixData[i, j] = i < j ? this[i, j] : 0;
            }
        }

        SquareMatrix rightTop = new SquareMatrix(matrixData);
        return (diagonal, leftBottom, rightTop);
    }

    public double? GetSpectralRadius()
    {
        List<Complex> eigenValues = GetEigenvalues();
        return eigenValues.Any() ? eigenValues.Select(Complex.Abs).Max() : null;
    }

    public bool IsTridiagonal()
    {
        if (Size == 1) return true;
        int[] countNonZeroItemsInRows = _data.Select(row => row.Count(value => value != 0)).ToArray();
        if (countNonZeroItemsInRows.Length == 2)
        {
            return countNonZeroItemsInRows[0] == 2 && countNonZeroItemsInRows[1] == 2;
        }

        return countNonZeroItemsInRows[0] == 2 && countNonZeroItemsInRows[^1] == 2 &&
               countNonZeroItemsInRows[1..^1].All(value => value == 3);
    }

    public bool IsDiagonalPredominance()
    {
        bool result = true;
        for (int i = 0; i < Size; i++)
        {
            List<double> row = _data[i];
            double diagonal = row[i];
            double sumOther = row
                .Where((value, index) => index != i)
                .Select(Math.Abs)
                .Sum();
            if (Math.Abs(diagonal) < sumOther) result = false;
        }

        return result;
    }

    public List<Complex> GetEigenvalues()
    {
        LambdaExpression[,] matrixWithLambda = GetNewRepresentationOfMatrix();
        return LinearSystemMath.SolveMatrixWithLambda(matrixWithLambda);
    }

    private LambdaExpression[,] GetNewRepresentationOfMatrix()
    {
        LambdaExpression[,] result = new LambdaExpression[Size, Size];
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                result[i, j] = i == j
                    ? new LambdaExpression(new LambdaDigit(this[i, j], 0)) - new LambdaExpression(new LambdaDigit(1, 1))
                    : new LambdaExpression(new LambdaDigit(this[i, j], 0));
            }
        }

        return result;
    }

    public double GetDeterminant()
    {
        switch (Size)
        {
            case 1: return this[0, 0];
            case 2: return this[0, 0] * this[1, 1] - this[1, 0] * this[0, 1];
            default:
                double determinant = 0;
                for (int column = 0; column < Size; column++)
                {
                    determinant += (column % 2 == 0 ? 1 : -1) * this[0, column] *
                                   GetMatrixWithoutRowAndColumn(0, column).GetDeterminant();
                }

                return determinant;
        }
    }

    private SquareMatrix GetMatrixWithoutRowAndColumn(int indexRow, int indexColumn)
    {
        List<List<double>> newData = _data.Select(value => value.ToList()).ToList();
        newData.RemoveAt(indexRow);
        foreach (List<double> row in newData)
        {
            row.RemoveAt(indexColumn);
        }

        return new SquareMatrix(newData);
    }

    /// <summary> Создает алгебраическое дополнение </summary>
    /// <returns>Новая матрица - алгебраическое дополнение текущей</returns>
    public SquareMatrix GetAlgebraicAddition()
    {
        List<List<double>> newData = new List<List<double>>();
        for (int i = 0; i < Size; i++)
        {
            newData.Add(new List<double>());
            for (int j = 0; j < Size; j++)
            {
                newData[i].Add(((i + j) % 2 is 0 ? 1 : -1) *
                               GetMatrixWithoutRowAndColumn(i, j).GetDeterminant());
            }
        }

        return new SquareMatrix(newData);
    }

    /// <summary> Инвертирует текущую матрицу</summary>
    public SquareMatrix GetInvertedMatrix()
    {
        double determinant = GetDeterminant();
        SquareMatrix transposed = GetAlgebraicAddition().GetTranspose();
        double[,] dataInvertSquareMatrix = new double[Size, Size];
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                dataInvertSquareMatrix[i, j] = transposed[i, j] / determinant;
            }
        }

        return new SquareMatrix(dataInvertSquareMatrix);
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < Size; i++)
        {
            builder.AppendLine(string.Join(' ', _data[i]));
        }

        return builder.ToString();
    }
}