using AzureResourceProbes.API.Application.AzureStorage;
using Microsoft.AspNetCore.Mvc;

namespace AzureResourceProbes.API.Presentation.Controllers;

[ApiController]
[Route("azurestorage")]
public class AzureStorageTestController : ControllerBase
{
    private readonly BlobService _blobService;
    private readonly ILogger<CPUTestController> _logger;

    public AzureStorageTestController(
        BlobService blobService,
        ILogger<CPUTestController> logger)
    {
        _blobService = blobService ?? throw new ArgumentNullException(nameof(BlobService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("blobs/containers")]
    public async Task<IActionResult> BlobCreation()
    {
        await _blobService.CreateContainerAsync();

        return Ok();
    }

    [HttpDelete("blobs/containers")]
    public async Task<IActionResult> BlobDeletion()
    {
        await _blobService.DeleteContainerAsync();

        return Ok();
    }

    [HttpPost("blobs")]
    public async Task<IActionResult> BlobUpload()
    {
        await _blobService.UploadBlogAsync();

        return Ok();
    }

    // QUEUES

    [HttpPost("queues")]
    public async Task<IActionResult> QueueCreation()
    {
        await _blobService.CreateQueueAsync();

        return Ok();
    }

    [HttpDelete("queues")]
    public async Task<IActionResult> QueueDeletion()
    {
        await _blobService.DeleteQueueAsync();

        return Ok();
    }

    [HttpPost("queues/messages")]
    public async Task<IActionResult> QueueMessageCreation(string message)
    {
        await _blobService.SendQueueMessageAsync(message);

        return Ok();
    }

    [HttpGet("queues/messages")]
    public async Task<IActionResult> QueueMessageCreation()
    {
        var messages = await _blobService.GetQueueMessagesAsync();

        return Ok(messages);
    }

    // TABLES

    [HttpPost("tables")]
    public async Task<IActionResult> TableCreation()
    {
        await _blobService.CreateTableAsync();

        return Ok();
    }

    [HttpDelete("tables")]
    public async Task<IActionResult> TableDeletion()
    {
        await _blobService.DeleteTableAsync();

        return Ok();
    }

    [HttpPost("tables/entries")]
    public async Task<IActionResult> AddTableEntry(string entry)
    {
        await _blobService.AddTableEntryAsync(entry);

        return Ok();
    }
}
