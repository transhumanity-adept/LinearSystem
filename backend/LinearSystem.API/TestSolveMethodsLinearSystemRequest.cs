namespace LinearSystem.API;

public class TestSolveMethodsLinearSystemRequest
{
    public IEnumerable<LinearSystemDTO> Systems { get; set; }

    public TestSolveMethodsLinearSystemRequest(IEnumerable<LinearSystemDTO> systems)
    {
        Systems = systems;
    }
}