using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Entities;

namespace SearchService.Consumers
{
    public class BidPlacedConsumer : IConsumer<BidPlaced>
    {
        public async Task Consume(ConsumeContext<BidPlaced> context)
        {
            var auction =
                await DB.Find<Item>().OneAsync(context.Message.AuctionId)
                ?? throw new Exception("Auction not found");

            if (
                context.Message.BidStatus.Contains("Accepted")
                && auction.CurrentHighBid < context.Message.Amount
            )
            {
                auction.CurrentHighBid = context.Message.Amount;
                await auction.SaveAsync();
            }
        }
    }
}
