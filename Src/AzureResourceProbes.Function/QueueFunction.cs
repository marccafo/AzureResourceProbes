using System;
using System.Text;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureResourceProbes.Function
{
    public class QueueFunction
    {
        private readonly QueueServiceClient _queueServiceClient;
        private readonly ILogger<QueueFunction> _logger;

        public QueueFunction(QueueServiceClient queueServiceClient,
            ILogger<QueueFunction> logger)
        {
            _queueServiceClient = queueServiceClient ?? throw new ArgumentNullException(nameof(queueServiceClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Function(nameof(QueueFunction))]
        public async Task Run([QueueTrigger("queuetest", Connection = "AZURESTORAGE_CONNECTION_STRING")] QueueMessage message)
        {
            _logger.LogInformation($"Mensaje recibido de la cola: {message.MessageText}");

            string responseMessage = $"Procesado: {message.MessageText}";

            QueueClient queueClient = _queueServiceClient.GetQueueClient("queuetest-response");

            await queueClient.CreateIfNotExistsAsync();
            await queueClient.SendMessageAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes(responseMessage)));

            _logger.LogInformation($"Mensaje de respuesta enviado: {responseMessage}");
        }
    }
}
