using LinearSystem.Solve.Tools;
using System;

SquareMatrix matrix = new SquareMatrix(new double[,]
{
    {1, 2, 3},
    {4, 5, 6},
    {7, 8, 7}
});
var q= matrix.GetGramSchmidtProcessResult();
Console.WriteLine(q);
Console.WriteLine();
var r = q * matrix;
Console.WriteLine(r);
Console.WriteLine(q.GetTranspose()*r);
Console.WriteLine();