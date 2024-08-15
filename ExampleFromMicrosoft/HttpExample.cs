using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Abstractions;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace My.Functions
{
    public class Person
    {
        public string? firstname { get; set; }
        public string? lastname { get; set; }
    }
    public class HttpExample
    {
        private readonly ILogger<HttpExample> _logger;

        public HttpExample(ILogger<HttpExample> logger)
        {
            _logger = logger;
        }

        [Function("HttpExample")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);

            //var body = new StreamReader(req.Body).ReadToEnd();
            Person? person= JsonSerializer.Deserialize<Person>(requestBody);
           var query = req.Query;
           var name = query.First(c => c.Key == "name").Value;
           if (person is null || person.firstname is null || person.lastname is null){
            return new OkObjectResult($"Not a valid person!");
           }
            //_logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult($"Welcome {person.firstname} {person.lastname}!");
        }
    }
}
