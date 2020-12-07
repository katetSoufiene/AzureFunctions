using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServerlessFuncs;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;
using Microsoft.WindowsAzure.Storage;

namespace FunctionV2
{
     public static class UserApi
        {
            [FunctionName("CreateUser")]
            public static async Task<IActionResult> CreateUser(
                [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user")] HttpRequest req,
                [Table("people", Connection = "AzureWebJobsStorage")] IAsyncCollector<UserTableEntity> userTable,
                [Queue("people", Connection = "AzureWebJobsStorage")] IAsyncCollector<User> userQueue,
                ILogger log)
            {
                log.LogInformation("Creating a new user list item");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var input = JsonConvert.DeserializeObject<UserCreateModel>(requestBody);

                var user = new User() { Name = input.Name };
                await userTable.AddAsync(user.ToTableEntity());
                await userQueue.AddAsync(user);
                return new OkObjectResult(user);
            }

            [FunctionName("Getpeople")]
            public static async Task<IActionResult> Getpeople(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user")] HttpRequest req,
                [Table("people", Connection = "AzureWebJobsStorage")] CloudTable userTable,
                ILogger log)
            {
                log.LogInformation("Getting user list items");
                var query = new TableQuery<UserTableEntity>();
                var segment = await userTable.ExecuteQuerySegmentedAsync(query, null);
                return new OkObjectResult(segment.Select(Mappings.ToUser));
            }

            [FunctionName("GetUserById")]
            public static IActionResult GetUserById(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/{id}")] HttpRequest req,
                [Table("people", "USER", "{id}", Connection = "AzureWebJobsStorage")] UserTableEntity user,
                ILogger log, string id)
            {
                log.LogInformation("Getting user item by id");
                if (user == null)
                {
                    log.LogInformation($"Item {id} not found");
                    return new NotFoundResult();
                }
                return new OkObjectResult(user.ToUser());
            }

            [FunctionName("UpdateUser")]
            public static async Task<IActionResult> UpdateUser(
                [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "user/{id}")] HttpRequest req,
                [Table("people", Connection = "AzureWebJobsStorage")] CloudTable userTable,
                ILogger log, string id)
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var updated = JsonConvert.DeserializeObject<UserUpdateModel>(requestBody);
                var findOperation = TableOperation.Retrieve<UserTableEntity>("USER", id);
                var findResult = await userTable.ExecuteAsync(findOperation);
                if (findResult.Result == null)
                {
                    return new NotFoundResult();
                }
                var existingRow = (UserTableEntity)findResult.Result;
                existingRow.IsActive = updated.IsActive;
                if (!string.IsNullOrEmpty(updated.Name))
                {
                    existingRow.Name = updated.Name;
                }

                var replaceOperation = TableOperation.Replace(existingRow);
                await userTable.ExecuteAsync(replaceOperation);
                return new OkObjectResult(existingRow.ToUser());
            }

            [FunctionName("DeleteUser")]
            public static async Task<IActionResult> DeleteUser(
                [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "user/{id}")] HttpRequest req,
                [Table("people", Connection = "AzureWebJobsStorage")] CloudTable userTable,
                ILogger log, string id)
            {
                var deleteOperation = TableOperation.Delete(new TableEntity()
                { PartitionKey = "USER", RowKey = id, ETag = "*" });
                try
                {
                    var deleteResult = await userTable.ExecuteAsync(deleteOperation);
                }
                catch (StorageException e) when (e.RequestInformation.HttpStatusCode == 404)
                {
                    return new NotFoundResult();
                }
                return new OkResult();
            }
        }

  
}
