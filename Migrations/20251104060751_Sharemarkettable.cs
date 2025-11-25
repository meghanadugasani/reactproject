using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Sharemarketsimulation.Migrations
{
    /// <inheritdoc />
    public partial class Sharemarkettable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Symbol = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OpenPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HighPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LowPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PreviousClose = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Volume = table.Column<long>(type: "bigint", nullable: false),
                    MarketCap = table.Column<long>(type: "bigint", nullable: false),
                    PriceChange = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceChangePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<int>(type: "int", nullable: false),
                    KYCStatus = table.Column<int>(type: "int", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockPriceHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockId = table.Column<int>(type: "int", nullable: false),
                    OpenPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HighPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LowPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ClosePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Volume = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockPriceHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockPriceHistories_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    StockId = table.Column<int>(type: "int", nullable: false),
                    OrderType = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StopLoss = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Target = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Portfolios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    StockId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    AveragePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalInvestment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrentValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProfitLoss = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProfitLossPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portfolios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Portfolios_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Portfolios_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FileSize = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedBy = table.Column<int>(type: "int", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDocuments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserDocuments_Users_VerifiedBy",
                        column: x => x.VerifiedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Watchlists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    StockId = table.Column<int>(type: "int", nullable: false),
                    AlertPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AlertType = table.Column<int>(type: "int", nullable: true),
                    IsAlertActive = table.Column<bool>(type: "bit", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Watchlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Watchlists_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Watchlists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    StockId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Commission = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Transactions_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Transactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Stocks",
                columns: new[] { "Id", "Category", "CompanyName", "CreatedAt", "CurrentPrice", "Description", "HighPrice", "IsActive", "LastUpdated", "LogoUrl", "LowPrice", "MarketCap", "OpenPrice", "PreviousClose", "PriceChange", "PriceChangePercent", "Symbol", "Volume" },
                values: new object[,]
                {
                    { 1, 1, "Apple Inc.", new DateTime(2025, 11, 4, 6, 7, 50, 972, DateTimeKind.Utc).AddTicks(48), 175.50m, "Technology company that designs and manufactures consumer electronics", 176.80m, true, new DateTime(2025, 11, 4, 6, 7, 50, 972, DateTimeKind.Utc).AddTicks(48), null, 173.90m, 2800000000000L, 174.20m, 174.00m, 1.50m, 0.86m, "AAPL", 45000000L },
                    { 2, 1, "Alphabet Inc.", new DateTime(2025, 11, 4, 6, 7, 50, 972, DateTimeKind.Utc).AddTicks(55), 142.30m, "Multinational technology conglomerate", 143.20m, true, new DateTime(2025, 11, 4, 6, 7, 50, 972, DateTimeKind.Utc).AddTicks(55), null, 140.80m, 1800000000000L, 141.50m, 141.00m, 1.30m, 0.92m, "GOOGL", 28000000L },
                    { 3, 1, "Microsoft Corporation", new DateTime(2025, 11, 4, 6, 7, 50, 972, DateTimeKind.Utc).AddTicks(61), 378.85m, "Multinational technology corporation", 380.50m, true, new DateTime(2025, 11, 4, 6, 7, 50, 972, DateTimeKind.Utc).AddTicks(62), null, 376.10m, 2800000000000L, 377.20m, 376.50m, 2.35m, 0.62m, "MSFT", 22000000L },
                    { 4, 1, "Tesla Inc.", new DateTime(2025, 11, 4, 6, 7, 50, 972, DateTimeKind.Utc).AddTicks(67), 248.50m, "Electric vehicle and clean energy company", 251.20m, true, new DateTime(2025, 11, 4, 6, 7, 50, 972, DateTimeKind.Utc).AddTicks(68), null, 244.30m, 790000000000L, 245.80m, 246.00m, 2.50m, 1.02m, "TSLA", 35000000L },
                    { 5, 2, "JPMorgan Chase & Co.", new DateTime(2025, 11, 4, 6, 7, 50, 972, DateTimeKind.Utc).AddTicks(73), 158.75m, "Multinational investment bank and financial services company", 159.80m, true, new DateTime(2025, 11, 4, 6, 7, 50, 972, DateTimeKind.Utc).AddTicks(73), null, 157.20m, 465000000000L, 157.90m, 158.00m, 0.75m, 0.47m, "JPM", 12000000L }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Balance", "CreatedAt", "Email", "FirstName", "IsActive", "KYCStatus", "LastName", "Password", "PhoneNumber", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1000000m, new DateTime(2025, 11, 4, 6, 7, 50, 971, DateTimeKind.Utc).AddTicks(9319), "admin@stockmarket.com", "Admin", true, 2, "User", "Admin@123", null, 2, null },
                    { 2, 50000m, new DateTime(2025, 11, 4, 6, 7, 50, 971, DateTimeKind.Utc).AddTicks(9470), "john.doe@email.com", "John", true, 2, "Doe", "User@123", "+1234567890", 1, null },
                    { 3, 75000m, new DateTime(2025, 11, 4, 6, 7, 50, 971, DateTimeKind.Utc).AddTicks(9475), "jane.smith@email.com", "Jane", true, 2, "Smith", "Broker@123", "+1234567891", 3, null }
                });

            migrationBuilder.InsertData(
                table: "Portfolios",
                columns: new[] { "Id", "AveragePrice", "CreatedAt", "CurrentValue", "ProfitLoss", "ProfitLossPercent", "Quantity", "StockId", "TotalInvestment", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 1, 170.00m, new DateTime(2025, 11, 4, 6, 7, 50, 972, DateTimeKind.Utc).AddTicks(139), 1755.00m, 55.00m, 3.24m, 10, 1, 1700.00m, null, 2 },
                    { 2, 140.00m, new DateTime(2025, 11, 4, 6, 7, 50, 972, DateTimeKind.Utc).AddTicks(142), 711.50m, 11.50m, 1.64m, 5, 2, 700.00m, null, 2 }
                });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "Commission", "NetAmount", "Notes", "OrderId", "Price", "Quantity", "StockId", "Tax", "TotalAmount", "TransactionDate", "TransactionType", "UserId" },
                values: new object[,]
                {
                    { 1, 5.00m, 1705.85m, "Initial purchase", null, 170.00m, 10, 1, 0.85m, 1700.00m, new DateTime(2025, 10, 28, 6, 7, 50, 972, DateTimeKind.Utc).AddTicks(273), 1, 2 },
                    { 2, 3.50m, 703.85m, "Google stock purchase", null, 140.00m, 5, 2, 0.35m, 700.00m, new DateTime(2025, 10, 30, 6, 7, 50, 972, DateTimeKind.Utc).AddTicks(285), 1, 2 }
                });

            migrationBuilder.InsertData(
                table: "Watchlists",
                columns: new[] { "Id", "AddedAt", "AlertPrice", "AlertType", "IsAlertActive", "Notes", "StockId", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 11, 4, 6, 7, 50, 972, DateTimeKind.Utc).AddTicks(208), 380.00m, 1, true, "Buy when it crosses 380", 3, 2 },
                    { 2, new DateTime(2025, 11, 4, 6, 7, 50, 972, DateTimeKind.Utc).AddTicks(211), null, null, false, "Monitoring Tesla for long term", 4, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_StockId",
                table: "Orders",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Portfolios_StockId",
                table: "Portfolios",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_Portfolios_UserId_StockId",
                table: "Portfolios",
                columns: new[] { "UserId", "StockId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockPriceHistories_StockId_Date",
                table: "StockPriceHistories",
                columns: new[] { "StockId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_Symbol",
                table: "Stocks",
                column: "Symbol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_OrderId",
                table: "Transactions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_StockId",
                table: "Transactions",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDocuments_UserId",
                table: "UserDocuments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDocuments_VerifiedBy",
                table: "UserDocuments",
                column: "VerifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Watchlists_StockId",
                table: "Watchlists",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_Watchlists_UserId_StockId",
                table: "Watchlists",
                columns: new[] { "UserId", "StockId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Portfolios");

            migrationBuilder.DropTable(
                name: "StockPriceHistories");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "UserDocuments");

            migrationBuilder.DropTable(
                name: "Watchlists");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
