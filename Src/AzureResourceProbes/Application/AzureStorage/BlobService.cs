using Azure.Storage.Blobs;

namespace AzureResourceProbes.Application.AzureStorage;

public class BlobService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<BlobService> _logger;

    private const string _containerName = "containertest";

    public BlobService(
        IConfiguration configuration,
        ILogger<BlobService> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task CreateContainerAsync()
    {
        var connectionString = _configuration.GetValue<string>("AzureStorageBlobs:ConnectionString");

        var blobServiceClient = new BlobServiceClient(connectionString);

        var containerClient = await blobServiceClient.CreateBlobContainerAsync(_containerName);
    }

    public async Task DeleteContainerAsync()
    {
        var connectionString = _configuration.GetValue<string>("AzureStorageBlobs:ConnectionString");

        var blobServiceClient = new BlobServiceClient(connectionString);

        await blobServiceClient.DeleteBlobContainerAsync(_containerName);
    }

    public async Task UploadBlogAsync()
    {
        var localPath = "temp";

        Directory.CreateDirectory(localPath);
        
        var fileName = "test.txt";

        var localFilePath = Path.Combine(localPath, fileName);

        await File.WriteAllTextAsync(localFilePath, "Hello, World!");

        var connectionString = _configuration.GetValue<string>("AzureStorageBlobs:ConnectionString");

        var blobServiceClient = new BlobServiceClient(connectionString);

        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

        var blobClient = containerClient.GetBlobClient(fileName);

        _logger.LogInformation("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

        await blobClient.UploadAsync(localFilePath, true);

        Directory.Delete(localPath, true);
    }

    public async Task<Stream> DownloadBlobAsync()
    {
        var localPath = "temp";

        Directory.CreateDirectory(localPath);

        var fileName = "test.txt";

        var localFilePath = Path.Combine(localPath, fileName);

        await File.WriteAllTextAsync(localFilePath, "Hello, World!");

        var connectionString = _configuration.GetValue<string>("AzureStorageBlobs:ConnectionString");

        var blobServiceClient = new BlobServiceClient(connectionString);

        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

        var blobClient = containerClient.GetBlobClient(fileName);

        _logger.LogInformation("Downloading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

        var blob = await blobClient.DownloadStreamingAsync();

        Directory.Delete(localPath, true);

        return blob.Value.Content;
    }
}
