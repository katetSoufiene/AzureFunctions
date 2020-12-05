using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FunctionAppStorageTable
{
    public static class PersonApi
    {
        [FunctionName("Add")]
        public static async Task<IActionResult> Add([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Person")] HttpRequest req,
            [Table("Person", Connection = "AzureWebJobsStorage")] IAsyncCollector<PersonTableEntity> personTable
          , ILogger log)
        {
            log.LogInformation(".... Add Person ....");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<PersonCreateModel>(requestBody);

            var person = new Person() { Name = input.Name };
            await personTable.AddAsync(person.PersonTableEntity());

            return new OkObjectResult(person);
        }


        [FunctionName("Get")]
        public static async Task<IActionResult> Get(
         [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Person")] HttpRequest req,
          [Table("Person", Connection = "AzureWebJobsStorage")] CloudTable personTable,
         ILogger log)
        {
            log.LogInformation("Get  all Person.");

            var query = new TableQuery<PersonTableEntity>();
            var segment = personTable.ExecuteQuerySegmentedAsync(query, null);

            return new OkObjectResult(segment.Result.Select(Mappings.ToPerson));
        }


        [FunctionName("GetById")]
        public static async Task<IActionResult> GetById(
       [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Person/{id}")] HttpRequest req,
       [Table("Person", "PERSON", "{id}", Connection = "AzureWebJobsStorage")] PersonTableEntity person,
       ILogger log, string id)
        {
            log.LogInformation("Get Person.");

            if (person == null) return new NotFoundResult();

            return new OkObjectResult(person.ToPerson());
        }


        [FunctionName("Update")]
        public static async Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Person/{id}")] HttpRequest req,
            [Table("Person", Connection = "AzureWebJobsStorage")] CloudTable personTable,
            ILogger log, string id)
        {
            log.LogInformation("C# Update Person.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updated = JsonConvert.DeserializeObject<PersonUpdateModel>(requestBody);
            var findOperation = TableOperation.Retrieve<PersonTableEntity>("PERSON", id);

            var findResult = await personTable.ExecuteAsync(findOperation);
            if (findResult.Result == null)
            {

                return new NotFoundResult();
            }
            var existingRow = (PersonTableEntity)findResult.Result;
            existingRow.IsActive = updated.IsActive;

            if (!string.IsNullOrEmpty(updated.Name))
            {
                existingRow.Name = updated.Name;
            }

            var replaceOperation = TableOperation.Replace(existingRow);
            await personTable.ExecuteAsync(replaceOperation);

            return new OkObjectResult(existingRow.ToPerson());
        }

        [FunctionName("Delete")]
        public static async Task<IActionResult> Delete(
           [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Person/{id}")] HttpRequest req,
            [Table("Person", Connection = "AzureWebJobsStorage")] CloudTable personTable,
           ILogger log, string id)
        {
            log.LogInformation("C# Delete Person.");
            var delateOperation = TableOperation.Delete(new TableEntity()
            {
                PartitionKey = "PERSON",
                RowKey = id,
                ETag = "*"
            });

            try
            {
                var deleteResult = await personTable.ExecuteAsync(delateOperation);
            }
            catch (StorageException e) when (e.RequestInformation.HttpStatusCode == 404)
            {

                return new NotFoundResult();
            }
            return new OkResult();
        }
    }
}
