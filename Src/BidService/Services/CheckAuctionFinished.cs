using BidService.Entities;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace BidService.Services
{
    public class CheckAuctionFinished(IServiceProvider services) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckAuctions(stoppingToken);

                await Task.Delay(5000, stoppingToken);
            }
        }

        private async Task CheckAuctions(CancellationToken stoppingToken)
        {
            var finishedAuctions = await DB.Find<Auction>()
                .Match(x => x.AuctionEnd <= DateTime.UtcNow)
                .Match(x => !x.Finished)
                .ExecuteAsync(stoppingToken);

            if (finishedAuctions.Count == 0)
            {
                return;
            }

            using var scope = services.CreateScope();
            var endpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            foreach (var auction in finishedAuctions)
            {
                auction.Finished = true;
                await auction.SaveAsync(null, stoppingToken);

                var winningBid = await DB.Find<Bid>()
                    .Match(a => a.AuctionId == auction.ID)
                    .Match(b => b.BidStatus == BidStatus.Accepted)
                    .Sort(x => x.Descending(s => s.Amount))
                    .ExecuteFirstAsync(stoppingToken);

                await endpoint.Publish(
                    new AuctionFinished
                    {
                        ItemSold = winningBid != null,
                        AuctionId = auction.ID,
                        Winner = winningBid?.Bidder,
                        Amount = winningBid?.Amount,
                        Seller = auction.Seller,
                    },
                    stoppingToken
                );
            }
        }
    }
}
