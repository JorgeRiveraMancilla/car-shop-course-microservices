using AuctionService.Data;
using AuctionService.Entities;
using Grpc.Core;

namespace AuctionService.Services
{
    public class GrpcAuctionService(DataContext dbContext) : GrpcAuction.GrpcAuctionBase
    {
        public override async Task<GrpcAuctionResponse> GetAuction(
            GetAuctionRequest request,
            ServerCallContext context
        )
        {
            Auction auction =
                await dbContext.Auctions.FindAsync(Guid.Parse(request.Id))
                ?? throw new RpcException(new Grpc.Core.Status(StatusCode.NotFound, "Not found"));

            GrpcAuctionResponse response = new()
            {
                Auction = new GrpcAuctionModel
                {
                    AuctionEnd = auction.AuctionEnd.ToString(),
                    Id = auction.Id.ToString(),
                    ReservePrice = auction.ReservePrice,
                    Seller = auction.Seller,
                },
            };

            return response;
        }
    }
}
