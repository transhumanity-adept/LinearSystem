using LinearSystem.API;
using LinearSystem.Solve;
using LinearSystem.Solve.Exceptions;
using LinearSystem.Solve.Tools;

var builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddApiVersioning();
services.AddCors();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseCors(builder => builder.AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader());

app.MapPost("/api/v1/solve", SolveHandler);
async Task<IResult> SolveHandler(SolveLinearSystemRequest request)
{
    try
    {
        SquareMatrix a = new SquareMatrix(request.System.MatrixA);
        VectorColumn b = new VectorColumn(request.System.VectorB);
        SolveMethods methodType = Enum.Parse<SolveMethods>(request.MethodType);
        CancellationTokenSource source = new CancellationTokenSource();
        CancellationToken token = source.Token;
        LinearSystemSolveWithStepMethodResult result = LinearSystemSolveBuilder.CreateIterative(methodType).Calculate(a, b, request.System.Error, token, null);
        return Results.Ok(result);
    }
    catch (Exception exception)
    {
        return Results.BadRequest(exception.Message);
    }
}

app.MapPost("/api/v1/solvetest", SolveTestHandler);
async Task<IResult> SolveTestHandler(TestSolveMethodsLinearSystemRequest request)
{
    try
    {
        List<(SquareMatrix a, VectorColumn b, double error, VectorColumn? initial)> systems =
            new List<(SquareMatrix a, VectorColumn b, double error, VectorColumn? initial)>();
        foreach (LinearSystemDTO system in request.Systems)
        {
            systems.Add((new SquareMatrix(system.MatrixA), new VectorColumn(system.VectorB), system.Error, null));
        }

        CancellationTokenSource source = new CancellationTokenSource();
        CancellationToken token = source.Token;
        var task = LinearSystemSolveBuilder.RunTest(systems, token);
        if (task.Wait(TimeSpan.FromSeconds(3)))
        {
            return Results.Ok(task.Result);
        }
        else {
            source.Cancel();
            return Results.BadRequest("Canceled");
        }
    }
    catch (Exception exception)
    {
        return Results.BadRequest(exception.Message);
    }
}

app.Run();