using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using ServerlessFuncsV2;

namespace FunctionV2
{
    public static class ScheduledFunction
    {
        [FunctionName("ScheduledFunction")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer,
        [Table("users", Connection = "AzureWebJobsStorage")] CloudTable userTable,
            ILogger log)
        {
            var query = new TableQuery<UserTableEntity>();
            var segment = await userTable.ExecuteQuerySegmentedAsync(query, null);
            var deleted = 0;
            foreach (var user in segment)
            {
                if (! user.IsActive)
                {
                    await userTable.ExecuteAsync(TableOperation.Delete(user));
                    deleted++;
                }
            }
            log.LogInformation($"Deleted {deleted} items at {DateTime.Now}");
        }
    }
}
