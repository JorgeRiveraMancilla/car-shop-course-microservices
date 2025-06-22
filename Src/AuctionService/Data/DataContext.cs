using AuctionService.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data
{
    public class DataContext(DbContextOptions options) : DbContext(options)
    {
        public required DbSet<Auction> Auctions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();

            modelBuilder
                .Entity<Auction>()
                .HasOne(x => x.Item)
                .WithOne(x => x.Auction)
                .HasForeignKey<Item>(x => x.AuctionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Item>().HasIndex(x => x.AuctionId).IsUnique();
            modelBuilder.Entity<Auction>().Navigation(x => x.Item).IsRequired();
            modelBuilder.Entity<Item>().Navigation(x => x.Auction).IsRequired();
        }
    }
}
