using Microsoft.EntityFrameworkCore;
using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Data
{
    public class StockMarketDbContext : DbContext
    {
        public StockMarketDbContext(DbContextOptions<StockMarketDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Watchlist> Watchlists { get; set; }
        public DbSet<UserDocument> UserDocuments { get; set; }
        public DbSet<StockPriceHistory> StockPriceHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Role).HasConversion<int>();
                entity.Property(e => e.KYCStatus).HasConversion<int>();
            });

            // Stock Configuration
            modelBuilder.Entity<Stock>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Symbol).IsUnique();
                entity.Property(e => e.CurrentPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.OpenPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.HighPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.LowPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PreviousClose).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PriceChange).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PriceChangePercent).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Category).HasConversion<int>();
            });

            // Portfolio Configuration
            modelBuilder.Entity<Portfolio>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AveragePrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalInvestment).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CurrentValue).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ProfitLoss).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ProfitLossPercent).HasColumnType("decimal(18,2)");
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Portfolios)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
                    
                entity.HasOne(e => e.Stock)
                    .WithMany(s => s.Portfolios)
                    .HasForeignKey(e => e.StockId)
                    .OnDelete(DeleteBehavior.NoAction);
                    
                entity.HasIndex(e => new { e.UserId, e.StockId }).IsUnique();
            });

            // Order Configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.StopLoss).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Target).HasColumnType("decimal(18,2)");
                entity.Property(e => e.OrderType).HasConversion<int>();
                entity.Property(e => e.Status).HasConversion<int>();
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
                    
                entity.HasOne(e => e.Stock)
                    .WithMany(s => s.Orders)
                    .HasForeignKey(e => e.StockId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Transaction Configuration
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Commission).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Tax).HasColumnType("decimal(18,2)");
                entity.Property(e => e.NetAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TransactionType).HasConversion<int>();
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Transactions)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
                    
                entity.HasOne(e => e.Stock)
                    .WithMany(s => s.Transactions)
                    .HasForeignKey(e => e.StockId)
                    .OnDelete(DeleteBehavior.NoAction);
                    
                entity.HasOne(e => e.Order)
                    .WithMany()
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Watchlist Configuration
            modelBuilder.Entity<Watchlist>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AlertPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.AlertType).HasConversion<int>();
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Watchlists)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
                    
                entity.HasOne(e => e.Stock)
                    .WithMany(s => s.Watchlists)
                    .HasForeignKey(e => e.StockId)
                    .OnDelete(DeleteBehavior.NoAction);
                    
                entity.HasIndex(e => new { e.UserId, e.StockId }).IsUnique();
            });

            // UserDocument Configuration
            modelBuilder.Entity<UserDocument>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DocumentType).HasConversion<int>();
                entity.Property(e => e.Status).HasConversion<int>();
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Documents)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
                    
                entity.HasOne(e => e.VerifiedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.VerifiedBy)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // StockPriceHistory Configuration
            modelBuilder.Entity<StockPriceHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OpenPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.HighPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.LowPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ClosePrice).HasColumnType("decimal(18,2)");
                
                entity.HasOne(e => e.Stock)
                    .WithMany(s => s.PriceHistory)
                    .HasForeignKey(e => e.StockId)
                    .OnDelete(DeleteBehavior.NoAction);
                    
                entity.HasIndex(e => new { e.StockId, e.Date }).IsUnique();
            });

            // Seed Data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@stockmarket.com",
                    Password = "Admin@123",
                    Role = UserRole.Admin,
                    KYCStatus = KYCStatus.Approved,
                    Balance = 1000000,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new User
                {
                    Id = 2,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@email.com",
                    Password = "User@123",
                    PhoneNumber = "+1234567890",
                    Role = UserRole.User,
                    KYCStatus = KYCStatus.Approved,
                    Balance = 50000,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new User
                {
                    Id = 3,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@email.com",
                    Password = "Broker@123",
                    PhoneNumber = "+1234567891",
                    Role = UserRole.Broker,
                    KYCStatus = KYCStatus.Approved,
                    Balance = 75000,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );

            // Seed Stocks
            modelBuilder.Entity<Stock>().HasData(
                new Stock
                {
                    Id = 1,
                    Symbol = "AAPL",
                    CompanyName = "Apple Inc.",
                    Description = "Technology company that designs and manufactures consumer electronics",
                    CurrentPrice = 175.50m,
                    OpenPrice = 174.20m,
                    HighPrice = 176.80m,
                    LowPrice = 173.90m,
                    PreviousClose = 174.00m,
                    Volume = 45000000,
                    MarketCap = 2800000000000,
                    PriceChange = 1.50m,
                    PriceChangePercent = 0.86m,
                    Category = StockCategory.Technology,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                },
                new Stock
                {
                    Id = 2,
                    Symbol = "GOOGL",
                    CompanyName = "Alphabet Inc.",
                    Description = "Multinational technology conglomerate",
                    CurrentPrice = 142.30m,
                    OpenPrice = 141.50m,
                    HighPrice = 143.20m,
                    LowPrice = 140.80m,
                    PreviousClose = 141.00m,
                    Volume = 28000000,
                    MarketCap = 1800000000000,
                    PriceChange = 1.30m,
                    PriceChangePercent = 0.92m,
                    Category = StockCategory.Technology,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                },
                new Stock
                {
                    Id = 3,
                    Symbol = "MSFT",
                    CompanyName = "Microsoft Corporation",
                    Description = "Multinational technology corporation",
                    CurrentPrice = 378.85m,
                    OpenPrice = 377.20m,
                    HighPrice = 380.50m,
                    LowPrice = 376.10m,
                    PreviousClose = 376.50m,
                    Volume = 22000000,
                    MarketCap = 2800000000000,
                    PriceChange = 2.35m,
                    PriceChangePercent = 0.62m,
                    Category = StockCategory.Technology,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                },
                new Stock
                {
                    Id = 4,
                    Symbol = "TSLA",
                    CompanyName = "Tesla Inc.",
                    Description = "Electric vehicle and clean energy company",
                    CurrentPrice = 248.50m,
                    OpenPrice = 245.80m,
                    HighPrice = 251.20m,
                    LowPrice = 244.30m,
                    PreviousClose = 246.00m,
                    Volume = 35000000,
                    MarketCap = 790000000000,
                    PriceChange = 2.50m,
                    PriceChangePercent = 1.02m,
                    Category = StockCategory.Technology,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                },
                new Stock
                {
                    Id = 5,
                    Symbol = "JPM",
                    CompanyName = "JPMorgan Chase & Co.",
                    Description = "Multinational investment bank and financial services company",
                    CurrentPrice = 158.75m,
                    OpenPrice = 157.90m,
                    HighPrice = 159.80m,
                    LowPrice = 157.20m,
                    PreviousClose = 158.00m,
                    Volume = 12000000,
                    MarketCap = 465000000000,
                    PriceChange = 0.75m,
                    PriceChangePercent = 0.47m,
                    Category = StockCategory.Finance,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                }
            );

            // Seed Portfolio
            modelBuilder.Entity<Portfolio>().HasData(
                new Portfolio
                {
                    Id = 1,
                    UserId = 2,
                    StockId = 1,
                    Quantity = 10,
                    AveragePrice = 170.00m,
                    TotalInvestment = 1700.00m,
                    CurrentValue = 1755.00m,
                    ProfitLoss = 55.00m,
                    ProfitLossPercent = 3.24m,
                    CreatedAt = DateTime.UtcNow
                },
                new Portfolio
                {
                    Id = 2,
                    UserId = 2,
                    StockId = 2,
                    Quantity = 5,
                    AveragePrice = 140.00m,
                    TotalInvestment = 700.00m,
                    CurrentValue = 711.50m,
                    ProfitLoss = 11.50m,
                    ProfitLossPercent = 1.64m,
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Seed Watchlist
            modelBuilder.Entity<Watchlist>().HasData(
                new Watchlist
                {
                    Id = 1,
                    UserId = 2,
                    StockId = 3,
                    AlertPrice = 380.00m,
                    AlertType = AlertType.Above,
                    IsAlertActive = true,
                    AddedAt = DateTime.UtcNow,
                    Notes = "Buy when it crosses 380"
                },
                new Watchlist
                {
                    Id = 2,
                    UserId = 2,
                    StockId = 4,
                    AddedAt = DateTime.UtcNow,
                    Notes = "Monitoring Tesla for long term"
                }
            );

            // Seed Transactions
            modelBuilder.Entity<Transaction>().HasData(
                new Transaction
                {
                    Id = 1,
                    UserId = 2,
                    StockId = 1,
                    TransactionType = TransactionType.Buy,
                    Quantity = 10,
                    Price = 170.00m,
                    TotalAmount = 1700.00m,
                    Commission = 5.00m,
                    Tax = 0.85m,
                    NetAmount = 1705.85m,
                    TransactionDate = DateTime.UtcNow.AddDays(-7),
                    Notes = "Initial purchase"
                },
                new Transaction
                {
                    Id = 2,
                    UserId = 2,
                    StockId = 2,
                    TransactionType = TransactionType.Buy,
                    Quantity = 5,
                    Price = 140.00m,
                    TotalAmount = 700.00m,
                    Commission = 3.50m,
                    Tax = 0.35m,
                    NetAmount = 703.85m,
                    TransactionDate = DateTime.UtcNow.AddDays(-5),
                    Notes = "Google stock purchase"
                }
            );
        }
    }
}