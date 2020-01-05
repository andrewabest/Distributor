﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Confluent.Kafka;
using Serilog;

namespace Distributor.Kafka.Consumer
{
    class Program
    { 
        static async Task Main(string[] args)
        {
            ConfigureLogging();

            var container = LetThereBeIOC();

            try
            {
                //await container.Resolve<ControlPanel>().Run();

                var conf = new ConsumerConfig
                { 
                    GroupId = "dowork-consumer-group",
                    BootstrapServers = "localhost:29092",
                    

                    // Note: The AutoOffsetReset property determines the start offset in the event
                    // there are not yet any committed offsets for the consumer group for the
                    // topic/partitions of interest. By default, offsets are committed
                    // automatically, so in this example, consumption will only start from the
                    // earliest message in the topic 'my-topic' the first time you run the program.
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };

                using var c = new ConsumerBuilder<string, string>(conf).Build();
                c.Subscribe("DoWork");

                var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) => {
                    e.Cancel = true; // prevent the process from terminating.
                    cts.Cancel();
                };

                try
                {
                    while (true)
                    {
                        try
                        {
                            // TODO: How does this work in Kafka as opposed to Rabbit?
                            // TODO: What if I want to re-read a topic from the start, ignoring client offset?

                            var cr = c.Consume(cts.Token);
                            Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    c.Close();
                }
            }
            finally
            {
                container.Dispose();
            }
        }

        private static IContainer LetThereBeIOC()
        {
            var builder = new ContainerBuilder();

            //builder.RegisterType<ControlPanel>().SingleInstance();

            return builder.Build();
        }

        private static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("Hello World!");
        }
    }
}
