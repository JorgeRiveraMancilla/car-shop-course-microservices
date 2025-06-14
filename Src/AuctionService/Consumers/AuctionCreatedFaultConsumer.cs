using Contracts;
using MassTransit;

namespace AuctionService.Consumers
{
    public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
    {
        public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
        {
            ExceptionInfo exception = context.Message.Exceptions.First();

            if (exception.ExceptionType == "System.ArgumentException")
            {
                context.Message.Message.Model = "FooBar";
                await context.Publish(context.Message.Message);
            }
        }
    }
}
