using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
namespace My.Functions
{
    public class HttpExample
    {
        private readonly ILogger<HttpExample> _logger;

        public HttpExample(ILogger<HttpExample> logger)
        {
            _logger = logger;
        }
        public static async Task<IActionResult> Get(HttpRequest req)
        {
            using NpgsqlConnection connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("POSTGRES_CONNECTIONSTRING"));
        connection.Open();

        using NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM users", connection);

        using NpgsqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine(reader["firstname"]);
            // Use the fetched results
        }
            var query = req.Query;
            string? name = query.First(c => c.Key == "name").Value;
            return new OkObjectResult($"Welcome {name}!");
        }

        public static async Task<IActionResult> Post(HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);

            //var body = new StreamReader(req.Body).ReadToEnd();
            Person? person = JsonSerializer.Deserialize<Person>(requestBody);
            var query = req.Query;
            var name = query.First(c => c.Key == "name").Value;
            if (person is null || person.firstname is null || person.lastname is null)
            {
                return new OkObjectResult($"Not a valid person!");
            }
            //_logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult($"Welcome {person.firstname} {person.lastname}!");
        }

        [Function("HttpExample")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req, ExecutionContext context)
        {

            var value = Environment.GetEnvironmentVariable("POSTGRES_USERNAME");
            Console.WriteLine("Hej");
            if (req.Method == "GET")
            {
                return await Get(req);
            }
            else
            { return await Post(req); }
        }
    }
    public class Person
    {
        public string? firstname { get; set; }
        public string? lastname { get; set; }
    }
}
/*
using Npgsql;
using System;

class Program
{
    static void Main()
    {
        string connectionString = "Host=my_host;Port=port_number;Database=database_name;User Id=username;Password=password;";

        using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using NpgsqlCommand cmd = new NpgsqlCommand(“SELECT * FROM customers”, connection);

        using NpgsqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Console.WriteLine(reader["column_name"]);
            // Use the fetched results
        }
    }
}
*/