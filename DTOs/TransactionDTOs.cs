using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int StockId { get; set; }
        public string StockSymbol { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public int? OrderId { get; set; }
        public TransactionType TransactionType { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Commission { get; set; }
        public decimal Tax { get; set; }
        public decimal NetAmount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Notes { get; set; }
    }

    public class TransactionSummaryDto
    {
        public decimal TotalBuyAmount { get; set; }
        public decimal TotalSellAmount { get; set; }
        public decimal TotalCommission { get; set; }
        public decimal TotalTax { get; set; }
        public int TotalTransactions { get; set; }
        public List<TransactionDto> RecentTransactions { get; set; } = new List<TransactionDto>();
    }

    public class TransactionFilterDto
    {
        public int? StockId { get; set; }
        public TransactionType? TransactionType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}