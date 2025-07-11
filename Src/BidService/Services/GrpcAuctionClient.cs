using AuctionService;
using BidService.Entities;
using Grpc.Net.Client;

namespace BidService.Services
{
    public class GrpcAuctionClient(IConfiguration config)
    {
        public Auction? GetAuction(string id)
        {
            var channel = GrpcChannel.ForAddress(config["GrpcAuction"]!);
            var client = new GrpcAuction.GrpcAuctionClient(channel);
            var request = new GetAuctionRequest { Id = id };

            try
            {
                var reply = client.GetAuction(request);
                var auction = new Auction
                {
                    ID = reply.Auction.Id,
                    AuctionEnd = DateTime.Parse(reply.Auction.AuctionEnd),
                    Seller = reply.Auction.Seller,
                    ReservePrice = reply.Auction.ReservePrice,
                };

                return auction;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
