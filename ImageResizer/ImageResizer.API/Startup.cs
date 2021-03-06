using Azure.Storage.Queues;
using ImageResizer.API;
using ImageResizer.API.Models;
using ImageResizer.API.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

[assembly: FunctionsStartup(typeof(Startup))]
namespace ImageResizer.API
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.setting.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddLogging();

            builder.Services.AddSingleton<IConfiguration>(config);
            builder.Services.AddSingleton((s) => new CosmosTableService<ImageEntity>(config["StorageConnectionString"], config["TableName"]));
            builder.Services.AddSingleton((s) => new QueueClient(config["QueueStorageConnectionString"], config["QueueName"]));
        }
    }
}
