using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.DTOs
{
    public class PortfolioDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int StockId { get; set; }
        public string StockSymbol { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal TotalInvestment { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal ProfitLoss { get; set; }
        public decimal ProfitLossPercent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class PortfolioSummaryDto
    {
        public decimal TotalInvestment { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal TotalProfitLoss { get; set; }
        public decimal TotalProfitLossPercent { get; set; }
        public decimal AvailableBalance { get; set; }
        public int TotalStocks { get; set; }
        public List<PortfolioDto> Holdings { get; set; } = new List<PortfolioDto>();
    }

    public class PortfolioPerformanceDto
    {
        public decimal DayChange { get; set; }
        public decimal DayChangePercent { get; set; }
        public decimal WeekChange { get; set; }
        public decimal WeekChangePercent { get; set; }
        public decimal MonthChange { get; set; }
        public decimal MonthChangePercent { get; set; }
        public decimal YearChange { get; set; }
        public decimal YearChangePercent { get; set; }
    }
}