using System;
using System.Threading.Tasks;
using Distributor.Messaging;
using Distributor.Worker.Contracts;
using MassTransit;
using Serilog;

namespace Distributor.Worker.Consumers
{
    public class DoWorkConsumer : IConsumer<DoWork>
    {
        private readonly IEventPublisher<ValueCreated> _event;

        public DoWorkConsumer(IEventPublisher<ValueCreated> @event)
        {
            _event = @event;
        }

        public async Task Consume(ConsumeContext<DoWork> context)
        {
            Log.Information("{amount} of work was requested", context.Message.Iterations);

            await Task.Delay(TimeSpan.FromMilliseconds(context.Message.Iterations));
            
            // context.Publish(new ValueCreated(5.0m * context.Message.Iterations));
            await _event.PublishEvent(new ValueCreated(5.0m * context.Message.Iterations, Worker.Id));
        }
    }
}