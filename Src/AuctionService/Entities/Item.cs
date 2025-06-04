namespace AuctionService.Entities
{
    public class Item
    {
        public Guid Id { get; set; }
        public required string Make { get; set; }
        public required string Model { get; set; }
        public int Year { get; set; }
        public required string Color { get; set; }
        public int Mileage { get; set; }
        public required string ImageUrl { get; set; }
        public Auction Auction { get; set; } = null!;
        public Guid AuctionId { get; set; }
    }
}
