using LinearSystem.Solve;

namespace LinearSystem.API;

public class SolveLinearSystemRequest
{
    public SolveLinearSystemRequest(LinearSystemDTO system, string methodType)
    {
        System = system;
        MethodType = methodType;
    }

    public LinearSystemDTO System { get; set; }
    public string MethodType { get; set; }
}