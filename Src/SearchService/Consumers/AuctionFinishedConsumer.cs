using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Entities;

namespace SearchService.Consumers
{
    public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
    {
        public async Task Consume(ConsumeContext<AuctionFinished> context)
        {
            var auction =
                await DB.Find<Item>().OneAsync(context.Message.AuctionId)
                ?? throw new Exception("Auction not found");

            if (context.Message.ItemSold)
            {
                auction.Winner = context.Message.Winner;
                auction.SoldAmount = (int)context.Message.Amount!;
            }

            auction.Status = "Finish";
            await auction.SaveAsync();
        }
    }
}
