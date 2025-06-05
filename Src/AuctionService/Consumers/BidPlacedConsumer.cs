using AuctionService.Data;
using AuctionService.Entities;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers
{
    public class BidPlacedConsumer(DataContext dataContext) : IConsumer<BidPlaced>
    {
        private readonly DataContext _dataContext = dataContext;

        public async Task Consume(ConsumeContext<BidPlaced> context)
        {
            Auction? auction = await _dataContext.Auctions.FindAsync(
                Guid.Parse(context.Message.AuctionId)
            );

            if (
                auction != null
                && (
                    auction.CurrentHighBid == null
                    || context.Message.BidStatus.Contains("Accepted")
                        && context.Message.Amount > auction.CurrentHighBid
                )
            )
            {
                auction.CurrentHighBid = context.Message.Amount;
                await _dataContext.SaveChangesAsync();
            }
        }
    }
}
