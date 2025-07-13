using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Hubs;

namespace NotificationService.Consumers
{
    public class AuctionCreatedConsumer(IHubContext<NotificationHub> hubContext)
        : IConsumer<AuctionCreated>
    {
        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            await hubContext.Clients.All.SendAsync("AuctionCreated", context.Message);
        }
    }
}
