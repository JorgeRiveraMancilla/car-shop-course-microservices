using AuctionService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data
{
    public class DataContext(DbContextOptions options) : DbContext(options)
    {
        public required DbSet<Auction> Auctions { get; set; }
    }
}
