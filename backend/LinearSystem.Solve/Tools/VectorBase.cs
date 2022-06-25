using System.Collections;

namespace LinearSystem.Solve.Tools;

public abstract class VectorBase : IEnumerable<double>
{
    private readonly double[] _data;

    public int Size => _data.Length;

    protected VectorBase(IEnumerable<double> data)
    {
        _data = data.ToArray();
    }

    protected VectorBase(int size)
    {
        _data = new double[size];
    }

    public double GetScalarProduct(VectorBase vector)
    {
        return  this.Zip(vector, (a, b) => a * b).Sum();
    }

    public double this[int index]
    {
        get => _data[index];
        set => _data[index] = value;
    }

    public double GetEuclideanNorm() => Math.Sqrt(this.GetScalarProduct(this));
    public double GetMaximumNorm() => _data.Select(Math.Abs).Max();

    public IEnumerator<double> GetEnumerator()
    {
        foreach (var value in _data)
        {
            yield return value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}