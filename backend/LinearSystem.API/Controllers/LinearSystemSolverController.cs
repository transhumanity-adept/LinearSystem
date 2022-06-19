using Microsoft.AspNetCore.Mvc;

namespace LinearSystem.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("solver-api/v{version:apiVersion}/solver")]
[ProducesResponseType(StatusCodes.Status200OK)]
public class LinearSystemSolverController : ControllerBase
{
    [HttpGet]
    public IActionResult Test()
    {
        return Ok("Hello from API");
    }
}