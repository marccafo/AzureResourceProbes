using Azure;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using System.Text;

namespace AzureResourceProbes.API.Application.AzureStorage;

public class BlobService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<BlobService> _logger;

    private const string _containerName = "containertest";
    private const string _queueName = "queuetest";
    private const string _tableName = "tabletest";

    public BlobService(
        IConfiguration configuration,
        ILogger<BlobService> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region Blob methods

    public async Task CreateContainerAsync()
    {
        var connectionString = _configuration.GetValue<string>("AzureStorage:ConnectionString");

        var blobServiceClient = new BlobServiceClient(connectionString);

        var containerClient = await blobServiceClient.CreateBlobContainerAsync(_containerName);
    }

    public async Task DeleteContainerAsync()
    {
        var connectionString = _configuration.GetValue<string>("AzureStorage:ConnectionString");

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

        var connectionString = _configuration.GetValue<string>("AzureStorage:ConnectionString");

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

        var connectionString = _configuration.GetValue<string>("AzureStorage:ConnectionString");

        var blobServiceClient = new BlobServiceClient(connectionString);

        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

        var blobClient = containerClient.GetBlobClient(fileName);

        _logger.LogInformation("Downloading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

        var blob = await blobClient.DownloadStreamingAsync();

        Directory.Delete(localPath, true);

        return blob.Value.Content;
    }

    #endregion

    #region Queue methods

    public async Task CreateQueueAsync()
    {
        var connectionString = _configuration.GetValue<string>("AzureStorage:ConnectionString");

        var queueClient = new QueueClient(connectionString, _queueName);

        await queueClient.CreateAsync();
    }

    public async Task DeleteQueueAsync()
    {
        var connectionString = _configuration.GetValue<string>("AzureStorage:ConnectionString");

        var queueClient = new QueueClient(connectionString, _queueName);

        await queueClient.DeleteAsync();
    }

    public async Task SendQueueMessageAsync(string message)
    {
        var connectionString = _configuration.GetValue<string>("AzureStorage:ConnectionString");

        var queueClient = new QueueClient(connectionString, _queueName);

        var plainTextBytes = Encoding.UTF8.GetBytes(message);
        var base64string = Convert.ToBase64String(plainTextBytes);

        await queueClient.SendMessageAsync(base64string);
    }

    public async Task<IEnumerable<string>> GetQueueMessagesAsync()
    {
        var connectionString = _configuration.GetValue<string>("AzureStorage:ConnectionString");

        var queueClient = new QueueClient(connectionString, _queueName);

        var peekedMessages = await queueClient.PeekMessagesAsync(maxMessages: 10);

        var messages = peekedMessages.Value
            .Select(x =>
            {
                var base64EncodedBytes = Convert.FromBase64String(x.MessageText);

                return Encoding.UTF8.GetString(base64EncodedBytes);
            })
            .Where(x => x != null)
            .ToList();

        return messages;
    }

    #endregion

    #region Table methods

    public async Task CreateTableAsync()
    {
        var connectionString = _configuration.GetValue<string>("AzureStorage:ConnectionString");

        var tableServiceClient = new TableServiceClient(connectionString);

        var tableClient = tableServiceClient.GetTableClient(
            tableName: _tableName
        );

        await tableClient.CreateIfNotExistsAsync();
    }

    public async Task DeleteTableAsync()
    {
        var connectionString = _configuration.GetValue<string>("AzureStorage:ConnectionString");

        var tableServiceClient = new TableServiceClient(connectionString);

        var tableClient = tableServiceClient.GetTableClient(
            tableName: _tableName
        );

        await tableClient.DeleteAsync();
    }

    public async Task AddTableEntryAsync(string value)
    {
        var connectionString = _configuration.GetValue<string>("AzureStorage:ConnectionString");

        var tableServiceClient = new TableServiceClient(connectionString);

        var tableClient = tableServiceClient.GetTableClient(
            tableName: _tableName
        );

        var item = new Item()
        {
            CustomProperty = value,
            PartitionKey = "default-partition",
            RowKey = Random.Shared.Next().ToString(),
        };

        await tableClient.AddEntityAsync(item);
    }

    public class Item : ITableEntity
    {
        public string CustomProperty { get; set; } = null!;
        public string PartitionKey { get; set; } = null!;
        public string RowKey { get; set; } = null!;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }

    #endregion
}
