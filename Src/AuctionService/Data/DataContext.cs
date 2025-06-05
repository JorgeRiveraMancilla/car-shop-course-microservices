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
                .HasOne(a => a.Item)
                .WithOne(i => i.Auction)
                .HasForeignKey<Item>(i => i.AuctionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Item>().HasIndex(i => i.AuctionId).IsUnique();
            modelBuilder.Entity<Auction>().Navigation(a => a.Item).IsRequired();
            modelBuilder.Entity<Item>().Navigation(i => i.Auction).IsRequired();
        }
    }
}
