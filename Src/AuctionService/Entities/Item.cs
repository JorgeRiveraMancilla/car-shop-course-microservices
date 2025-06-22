namespace AuctionService.Entities
{
    public class Item
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Make { get; set; }
        public required string Model { get; set; }
        public required int Year { get; set; }
        public required string Color { get; set; }
        public required int Mileage { get; set; }
        public required string ImageUrl { get; set; }

        // Navigation properties
        public Guid AuctionId { get; set; }
        public Auction? Auction { get; set; }
    }
}
