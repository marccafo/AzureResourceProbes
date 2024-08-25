using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzureResourceProbes.Function
{
    public class Function1
    {
        [FunctionName("Function1")]
        public void Run([QueueTrigger("queuetest", Connection = "AZURESTORAGE_CONNECTION_STRING")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
