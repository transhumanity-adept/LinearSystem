namespace LinearSystem.API;

public class LinearSystemDTO
{
    public LinearSystemDTO(IEnumerable<IEnumerable<double>> matrixA, IEnumerable<double> vectorB, double error)
    {
        MatrixA = matrixA;
        VectorB = vectorB;
        Error = error;
    }

    public IEnumerable<IEnumerable<double>> MatrixA { get; set; }
    public IEnumerable<double> VectorB { get; set; }
    public double Error { get; set; }
}