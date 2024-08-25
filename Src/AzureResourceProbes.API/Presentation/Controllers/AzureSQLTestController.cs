using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AzureResourceProbes.API.Presentation.Controllers;

[ApiController]
public class AzureSQLTestController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<CPUTestController> _logger;

    public AzureSQLTestController(
        IConfiguration configuration,
        ILogger<CPUTestController> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("azuresql/check-connection")]
    public async Task<IActionResult> AzureSQLTest()
    {
        try
        {
            var connectionString = _configuration.GetValue<string>("AzureSQL:ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    return Ok("Conexión exitosa con la base de datos.");
                }
                else
                {
                    return StatusCode(500, "No se pudo establecer conexión con la base de datos.");
                }
            }
        }
        catch (SqlException ex)
        {
            return StatusCode(500, $"Error al conectar con la base de datos: {ex.Message}");
        }
    }
}
