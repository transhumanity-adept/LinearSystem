using LinearSystem.Solve.Tools;
using System;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Channels;
using LinearSystem.Solve;
using LinearSystem.Solve.Methods.GaussSeidel;
using LinearSystem.Solve.Methods.Jacobi;
using LinearSystem.Solve.Methods.Relaxation;

SquareMatrix matrix = new SquareMatrix(new double[,]
{
    {1, 2, 1},
    {2, 10, 1},
    {2, 2, 10},
});

VectorColumn b = new VectorColumn(new double[]
{
    1.2,
    1.3,
    1.4
});

var systems = new List<(SquareMatrix a, VectorColumn b, double error, VectorColumn? initial)>()
{
    (matrix, b, 0.001, null)
};

/*TestResult result = LinearSystemSolveBuilder.RunTest(systems).Result;

Console.WriteLine(JsonSerializer.Serialize(result));*/