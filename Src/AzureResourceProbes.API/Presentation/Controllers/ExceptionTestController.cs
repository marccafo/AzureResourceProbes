using Microsoft.AspNetCore.Mvc;

namespace AzureResourceProbes.API.Presentation.Controllers;

[ApiController]
public class ExceptionTestController : ControllerBase
{
    private readonly ILogger<CPUTestController> _logger;

    public ExceptionTestController(ILogger<CPUTestController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("exception")]
    public IActionResult ExceptionTest()
    {
        throw new Exception("Excepción del sistema provocada por el usuario de forma manual");
    }
}
