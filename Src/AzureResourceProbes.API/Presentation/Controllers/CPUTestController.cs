using Microsoft.AspNetCore.Mvc;

namespace AzureResourceProbes.API.Presentation.Controllers;

[ApiController]
public class CPUTestController : ControllerBase
{
    private readonly ILogger<CPUTestController> _logger;

    public CPUTestController(ILogger<CPUTestController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("cpu")]
    public async Task<IActionResult> CPUTest()
    {
        int parallelTasks = Environment.ProcessorCount * 2;

        var tasks = Enumerable.Range(0, parallelTasks).Select(_ => Task.Run(() =>
        {
            double result = 0;
            for (int i = 0; i < 1_000_000_000; i++)
            {
                result += Math.Sqrt(i);
            }
        }));

        await Task.WhenAll(tasks);

        return Ok();
    }
}
