using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DotComTrading.Models;

namespace DotComTrading.Data
{
    //Main database context used for integrating identity and application data
    public class DotComTradingDBContext : IdentityDbContext<ApplicationUser>
    {
        public DotComTradingDBContext(DbContextOptions<DotComTradingDBContext> options) : base(options) 
        { 
        }

        //Main entity tables
        public DbSet<Website> Websites { get; set; } = null!;
        public DbSet<Portfolio> Portfolios { get; set; } = null!;
        public DbSet<Holding> Holdings { get; set; } = null!;
        public DbSet<Trade> Trades { get; set; } = null!;
        public DbSet<WebsitePriceRecord> WebsitePriceRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Website>(website =>
            {
                website.HasKey(w => w.Id);
                website.Property(w => w.Name).HasMaxLength(40).IsRequired();
                website.Property(w => w.Domain).HasMaxLength(80).IsRequired();
                website.Property(w => w.Url).HasMaxLength(120);
                website.Property(w => w.Price).HasColumnType("decimal(11,2)");
                website.HasIndex(w => w.Domain).IsUnique();
            });

            modelBuilder.Entity<Portfolio>(portfolio =>
            {
                portfolio.HasKey(p => p.Id);
                portfolio.Property(p => p.Balance).HasColumnType("decimal(20,2)");
                portfolio.HasOne(p => p.User).WithOne(u => u.Portfolio).HasForeignKey<Portfolio>(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Holding>(holding =>
            {
                holding.HasKey(h => h.Id);
                holding.Property(h => h.NoShares).HasColumnType("decimal(18, 2)");
                holding.HasOne(h => h.Portfolio).WithMany(p => p.Holdings).HasForeignKey(h => h.PortfolioId).OnDelete(DeleteBehavior.Cascade);
                holding.HasOne(h => h.Website).WithMany().HasForeignKey(h => h.WebsiteId).OnDelete(DeleteBehavior.Cascade);
                holding.HasIndex(h => new { h.PortfolioId, h.WebsiteId }).IsUnique();
            });

            modelBuilder.Entity<Trade>(trade =>
            {
                trade.HasKey(t => t.Id);
                trade.Property(t => t.PricePerShare).HasColumnType("decimal(15, 2)");
                trade.Property(t => t.NoShares).HasColumnType("decimal(15, 2)");
                trade.Property(t => t.TradeType).HasMaxLength(8).IsRequired();
                trade.HasOne(t => t.Portfolio).WithMany(p => p.Trades).HasForeignKey(t => t.PortfolioId).OnDelete(DeleteBehavior.Cascade);
                trade.HasOne(t => t.Website).WithMany(w => w.Trades).HasForeignKey(t => t.WebsiteId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<WebsitePriceRecord>(record =>
            {
                record.HasKey(r => r.Id);
                record.Property(r => r.Price).HasColumnType("decimal(15, 2)");
                record.Property(r => r.TimeOfRecording).IsRequired();
                record.HasOne(r => r.Website).WithMany(w => w.PriceRecords).HasForeignKey(r => r.WebsiteId).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
