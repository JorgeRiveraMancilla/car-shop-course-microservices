using AuctionService.Data;
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
            var auction =
                await dbContext.Auctions.FindAsync(Guid.Parse(request.Id))
                ?? throw new RpcException(new Status(StatusCode.NotFound, "Not found"));

            var response = new GrpcAuctionResponse
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
