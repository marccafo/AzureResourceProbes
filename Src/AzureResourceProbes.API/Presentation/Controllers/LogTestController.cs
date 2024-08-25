using Microsoft.AspNetCore.Mvc;

namespace AzureResourceProbes.API.Presentation.Controllers;

[ApiController]
public class LogTestController : ControllerBase
{
    private readonly ILogger<CPUTestController> _logger;

    public LogTestController(ILogger<CPUTestController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("log")]
    public IActionResult LogTest()
    {
        _logger.LogTrace("LogTrace - Registro de prueba");
        _logger.LogInformation("LogInformation - Registro de prueba");
        _logger.LogWarning("LogWarning - Registro de prueba");

        var exception = new Exception("Exception de prueba");
        _logger.LogError(exception, "LogError - Registro de prueba");

        var criticalException = new Exception("Exception crítica de prueba");
        _logger.LogCritical(criticalException, "LogCritical - Registro de prueba");

        return Ok();
    }
}
