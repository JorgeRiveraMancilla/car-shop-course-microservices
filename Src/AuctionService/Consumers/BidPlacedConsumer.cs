using AuctionService.Data;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers
{
    public class BidPlacedConsumer(DataContext dataContext) : IConsumer<BidPlaced>
    {
        private readonly DataContext _dataContext = dataContext;

        public async Task Consume(ConsumeContext<BidPlaced> context)
        {
            var auction =
                await _dataContext.Auctions.FindAsync(context.Message.AuctionId)
                ?? throw new Exception("Auction not found");

            if (
                auction.CurrentHighBid == null
                || (
                    context.Message.BidStatus.Contains("Accepted")
                    && auction.CurrentHighBid < context.Message.Amount
                )
            )
            {
                auction.CurrentHighBid = context.Message.Amount;
                await _dataContext.SaveChangesAsync();
            }
        }
    }
}
