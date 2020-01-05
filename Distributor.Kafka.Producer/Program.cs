using System.Threading.Tasks;
using Autofac;
using Serilog;

namespace Distributor.Kafka.Producer
{
    class Program
    { 
        static async Task Main(string[] args)
        {
            ConfigureLogging();

            var container = LetThereBeIOC();

            try
            {
                await container.Resolve<ControlPanel>().Run();
            }
            finally
            {
                container.Dispose();
            }
        }

        private static IContainer LetThereBeIOC()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ControlPanel>().SingleInstance();

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
