using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace AzureResourceProbes.API.Presentation.Controllers;

[ApiController]
public class AzureOpenAITestController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CPUTestController> _logger;

    public AzureOpenAITestController(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<CPUTestController> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("azureopenai/generate-text")]
    public async Task<IActionResult> AzureSQLTest(string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            return BadRequest("El prompt no puede estar vacío.");
        }

        var openAIConfig = _configuration.GetSection("AzureOpenAI");
        var endpoint = openAIConfig["Endpoint"];
        var apiKey = openAIConfig["ApiKey"];

        var requestBody = new
        {
            prompt = prompt,
            max_tokens = 50,  // Puedes ajustar este valor según tus necesidades
            temperature = 0.7 // Controla la creatividad de las respuestas
        };

        var jsonRequestBody = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var response = await _httpClient.PostAsync($"{endpoint}openai/deployments/text-davinci-002/completions?api-version=2023-06-01-preview", content);

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            return Ok(jsonResponse);
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, $"Error al generar texto: {error}");
        }
    }
}
