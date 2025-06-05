namespace AuctionService.Entities
{
    public class Auction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int ReservePrice { get; set; } = 0;
        public required string Seller { get; set; }
        public string? Winner { get; set; }
        public int? SoldAmount { get; set; }
        public int? CurrentHighBid { get; set; }
        public required DateTime AuctionEnd { get; set; }
        public Status Status { get; set; } = Status.Live;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Item? Item { get; set; }
    }
}
