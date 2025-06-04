using AuctionService.Data;
using AuctionService.Entities;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers
{
    public class AuctionFinishedConsumer(DataContext dataContext) : IConsumer<AuctionFinished>
    {
        private readonly DataContext _dataContext = dataContext;

        public async Task Consume(ConsumeContext<AuctionFinished> context)
        {
            var auction =
                await _dataContext.Auctions.FindAsync(context.Message.AuctionId)
                ?? throw new Exception("Auction not found");

            if (context.Message.ItemSold)
            {
                auction.Winner = context.Message.Winner;
                auction.SoldAmount = context.Message.Amount;
            }

            if (auction.ReservePrice < auction.SoldAmount)
            {
                auction.Status = Status.Finish;
            }
            else
            {
                auction.Status = Status.NotFulfilled;
            }

            await _dataContext.SaveChangesAsync();
        }
    }
}
