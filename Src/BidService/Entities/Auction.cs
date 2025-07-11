using MongoDB.Entities;

namespace BidService.Entities
{
    public class Auction : Entity
    {
        public DateTime AuctionEnd { get; set; }
        public required string Seller { get; set; }
        public int ReservePrice { get; set; }
        public bool Finished { get; set; }
    }
}
