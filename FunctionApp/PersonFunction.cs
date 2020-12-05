using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace FunctionApp
{
    public static class PersonFunction
    {

        public static List<Person> items { get; set; } = new List<Person>();

        [FunctionName("Add")]
        public static async Task<IActionResult> Add(
          [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Person")] HttpRequest req,
          ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<PersonCreateModel>(requestBody);

            var Person = new Person() { Name = input.Name };
            items.Add(Person);

            return new OkObjectResult(Person);
        }


        [FunctionName("Get")]
        public static async Task<IActionResult> Get(
         [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Person")] HttpRequest req,
         ILogger log)
        {
            log.LogInformation("C# GetPersons.");

            return new OkObjectResult(items);
        }


        [FunctionName("GetById")]
        public static async Task<IActionResult> GetById(
       [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Person/{id}")] HttpRequest req,
       ILogger log, string id)
        {
            log.LogInformation("C# GetPersons.");

            var item = items.FirstOrDefault(item => item.Id == id);

            if (item == null) return new NotFoundResult();

            return new OkObjectResult(item);
        }


        [FunctionName("Update")]
        public static async Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Person/{id}")] HttpRequest req, ILogger log, string id)
        {
            log.LogInformation("C# UpdatePerson.");

            var Person = items.FirstOrDefault(item => item.Id == id);

            if (Person == null) return new NotFoundResult();
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updated = JsonConvert.DeserializeObject<PersonUpdateModel>(requestBody);

            Person.IsActive = updated.IsActive;

            if (!string.IsNullOrEmpty(updated.Name))
            {
                Person.Name = updated.Name;
            }

            return new OkObjectResult(Person);
        }

        [FunctionName("Delete")]
        public static async Task<IActionResult> Delete(
           [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Person/{id}")] HttpRequest req, ILogger log, string id)
        {
            log.LogInformation("C# UpdatePerson.");

            var Person = items.FirstOrDefault(item => item.Id == id);

            if (Person == null) return new NotFoundResult();
            items.Remove(Person);

            return new OkResult();
        }


    }
}
