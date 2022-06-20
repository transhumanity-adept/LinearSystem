using System.Collections;

namespace LinearSystem.Solve.Tools;

public abstract class VectorBase : IEnumerable<double>
{
    private readonly List<double> _data;

    public int Size => _data.Count;

    protected VectorBase(IEnumerable<double> data)
    {
        _data = data.ToList();
    }

    public double GetScalarProduct(VectorBase vector)
    {
        return  this.Zip(vector, (a, b) => a * b).Sum();
    }

    public double this[int index] => _data[index];

    public double GetEuclideanNorm() => Math.Sqrt(this.GetScalarProduct(this));

    public IEnumerator<double> GetEnumerator()
    {
        return _data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}