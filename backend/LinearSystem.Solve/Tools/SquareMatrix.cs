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
    public double this[int rowIndex, int columnIndex] => _data[rowIndex][columnIndex];
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
    public SquareMatrix GetGramSchmidtProcessResult()
    {
        List<VectorColumn> orthogonalVectors = new List<VectorColumn>()
        {
            this[0]
        };
        for (int k = 1; k < Size; k++)
        {
            VectorColumn currentVector = this[k];
            VectorColumn projectionSum = orthogonalVectors.Aggregate(currentVector.Zero,
                (sum, orthogonalVector) => sum + currentVector.GetProjectTo(orthogonalVector));
            orthogonalVectors.Add(currentVector - projectionSum);
        }

        return new SquareMatrix(orthogonalVectors.Select(vector => vector / vector.GetEuclideanNorm()));
    }
    public (SquareMatrix Q, SquareMatrix R) GetQRDecomposition()
    {
        SquareMatrix q = GetGramSchmidtProcessResult();
        return (q, q.GetTranspose() * this);
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