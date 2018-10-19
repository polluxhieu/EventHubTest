using System;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace EventsReceiver
{

    class Program
    {
        private static IConfigurationRoot _config;

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);
            _config = builder.Build();

            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            Console.WriteLine("Registering EventProcessor...");

            var eventHubConnection = _config["EventHubConnection"];
            var eventHubName = _config["EventHubName"];
            var storageContainerName = _config["StorageContainerName"];
            var storageAccountName = _config["StorageAccountName"];
            var storageAccountKey = _config["StorageAccountKey"];
            var storageConnectionString = _config["StorageConnectionString"];

            var eventProcessorHost = new EventProcessorHost(eventHubName, PartitionReceiver.DefaultConsumerGroupName, eventHubConnection, storageConnectionString, storageContainerName);
            await eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>();

            Console.WriteLine("Receiving. Press ENTER to stop worker.");
            Console.ReadLine();

            await eventProcessorHost.UnregisterEventProcessorAsync();
        }
    }
}
