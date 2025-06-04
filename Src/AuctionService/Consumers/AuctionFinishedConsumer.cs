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
                await _dataContext.Auctions.FindAsync(Guid.Parse(context.Message.AuctionId))
                ?? throw new InvalidOperationException(
                    $"Auction with ID {context.Message.AuctionId} not found."
                );

            if (context.Message.ItemSold)
            {
                auction.Winner = context.Message.Winner;
                auction.SoldAmount = context.Message.Amount;
            }

            auction.Status =
                auction.SoldAmount > auction.ReservePrice ? Status.Finish : Status.NotFulfilled;

            await _dataContext.SaveChangesAsync();
        }
    }
}
