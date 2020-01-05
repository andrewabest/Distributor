using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;
using Serilog;

namespace Distributor.Kafka.Producer
{
    public class ControlPanel
    {

        private readonly Dictionary<string, Func<ControlPanel, Task>> _actions = new Dictionary<string, Func<ControlPanel, Task>>
        {
            { "1", controlPanel => controlPanel.Work() }
        };

        public async Task Run()
        {
            do
            {
                Console.WriteLine("Press 1 to Start Work");
                Console.WriteLine("Enter quit to exit");
                Console.Write("> ");
                var value = Console.ReadLine();

                if (value == "quit") break;

                if (_actions.ContainsKey(value))
                {
                    await _actions[value](this);
                }
                else
                {
                    Log.Information("Could not find action with value {value}", value);
                }

            }
            while (true);
        }

        public async Task Work()
        {
            while (true)
            {
                Log.Information("Requesting {amount} of work - mosh!", 1000);
                await Task.Delay(TimeSpan.FromSeconds(3));

                var config = new ProducerConfig { BootstrapServers = "localhost:29092" };

                using var producer = new ProducerBuilder<string, string>(config).Build();

                var deliveryReport = await producer.ProduceAsync(
                    "DoWork", new Message<string, string> { Key = Guid.NewGuid().ToString(), Value = "Mosh!" });

                Console.WriteLine($"Work delivered to: {deliveryReport.TopicPartitionOffset}");
            }
        }
    }
}