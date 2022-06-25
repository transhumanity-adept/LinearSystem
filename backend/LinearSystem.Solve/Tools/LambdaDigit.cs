namespace LinearSystem.Solve.Tools;

public readonly struct LambdaDigit
{
    public double Coefficient { get; }
    public int LambdaDegree { get; }
    public LambdaDigit(double coefficient, int lambdaDegree)
    {
        Coefficient = coefficient;
        LambdaDegree = lambdaDegree;
    }
    
    public override string ToString()
    {
        return $"({Coefficient};{LambdaDegree})";
    }
}