using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Configuration;

namespace EventsSender
{
    internal class Program
    {
        private static IConfigurationRoot _config;
        private static EventHubClient _client;

        private static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);
            _config = builder.Build();

            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            var eventHubConnection = _config["EventHubConnection"];
            var eventHubName = _config["EventHubName"];
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(eventHubConnection) { EntityPath = eventHubName };
            _client = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            await SendMessageToEventHub(5);
            await _client.CloseAsync();

            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }

        private static async Task SendMessageToEventHub(int number)
        {
            for (int i = 0; i < number; ++i)
            {
                try
                {
                    var msg = $"Message {i}";
                    Console.WriteLine($"Sending {msg}");
                    await _client.SendAsync(new EventData(Encoding.UTF8.GetBytes(msg)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now} EXCEPTION: {ex.Message}");
                }
                await Task.Delay(10);
            }

            Console.WriteLine($"{number} messages sent.");
        }
    }
}
