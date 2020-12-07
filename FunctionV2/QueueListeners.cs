using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using ServerlessFuncsV2;

namespace FunctionV2
{
    public static class QueueListeners
    {
        [FunctionName("QueueListeners")]
        public static async Task Run([QueueTrigger("users", Connection = "AzureWebJobsStorage")] User user,
            [Blob("users", Connection = "AzureWebJobsStorage")] CloudBlobContainer container,
            ILogger log)
        {
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlockBlobReference($"{user.Id}.txt");
            await blob.UploadTextAsync($"Created a new User: {user.Name}");
            log.LogInformation($"Queue trigger function processed: {user.Name}");
        }
    }
}
