namespace LinearSystem.Solve.Tools;

public class VectorColumn : VectorBase
{
    private VectorColumn _zero;
    public VectorColumn Zero => _zero ??= new VectorColumn(Enumerable.Repeat<double>(0.0, Size));
    public VectorColumn(IEnumerable<double> data) : base(data)
    {
    }

    public VectorColumn(int size) : base(size)
    {
    }

    public static VectorColumn operator *(VectorColumn vector, double value)
    {
        return new VectorColumn(vector.Select(vectorValue => vectorValue * value));
    }

    public VectorColumn GetProjectTo(VectorColumn vector)
    {
        return vector * (this.GetScalarProduct(vector) / Math.Pow(vector.GetEuclideanNorm(), 2));
    }
    
    public static VectorColumn operator +(VectorColumn vectorOne, VectorColumn vectorTwo)
    {
        return new VectorColumn(vectorOne.Zip(vectorTwo, (a, b) => a + b));
    }
    
    public static VectorColumn operator -(VectorColumn vector)
    {
        return new VectorColumn(vector.Select(value => -value));
    }
    
    public static VectorColumn operator -(VectorColumn vectorOne, VectorColumn vectorTwo)
    {
        return vectorOne + -vectorTwo;
    }

    public static VectorColumn operator /(VectorColumn vector, double value)
    {
        return new VectorColumn(vector.Select(vectorValue => vectorValue / value));
    }
}