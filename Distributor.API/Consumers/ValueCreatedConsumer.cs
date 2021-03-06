﻿using System.Threading.Tasks;
using Distributor.Worker.Contracts;
using MassTransit;
using Serilog;

namespace Distributor.API.Consumers
{
    public class ValueCreatedConsumer : IConsumer<ValueCreated>
    {
        public Task Consume(ConsumeContext<ValueCreated> context)
        {
            Log.Information("{value} of value was created by {workerId}", context.Message.Amount, context.Message.WorkerId.ToString("N"));

            return Task.CompletedTask;
        }
    }
}