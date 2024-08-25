using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace AzureResourceProbes.API.Presentation.Controllers;

[ApiController]
public class AzureCosmosDBTestController : ControllerBase
{
    private readonly CosmosClient _cosmosClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CPUTestController> _logger;

    public AzureCosmosDBTestController(
        IConfiguration configuration,
        ILogger<CPUTestController> logger)
    {
        var cosmosSettings = configuration.GetSection("AzureCosmosDb");
        var accountEndpoint = cosmosSettings["AccountEndpoint"];
        var accountKey = cosmosSettings["AccountKey"];

        _cosmosClient = new CosmosClient(accountEndpoint, accountKey);
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("azurecosmosdb/check-connection")]
    public async Task<IActionResult> AzureSQLTest(string databaseName, string containerName)
    {
        try
        {
            Database database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
            Container container = await database.CreateContainerIfNotExistsAsync(containerName, "/partitionKeyPath");

            return Ok("Conexión exitosa con Cosmos DB.");
        }
        catch (CosmosException ex)
        {
            return StatusCode(500, $"Error al conectar con Cosmos DB: {ex.Message}");
        }
    }
}
