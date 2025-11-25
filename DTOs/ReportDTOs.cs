namespace Sharemarketsimulation.DTOs
{
    public class PnLReportDto
    {
        public decimal TotalInvestment { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal RealizedPnL { get; set; }
        public decimal UnrealizedPnL { get; set; }
        public decimal TotalPnL { get; set; }
        public decimal TotalPnLPercent { get; set; }
        public DateTime ReportDate { get; set; }
        public List<StockPnLDto> StockBreakdown { get; set; } = new List<StockPnLDto>();
    }

    public class StockPnLDto
    {
        public string Symbol { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal Investment { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal PnL { get; set; }
        public decimal PnLPercent { get; set; }
    }

    public class UserActivityReportDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public int ExecutedOrders { get; set; }
        public decimal TotalVolume { get; set; }
        public decimal TotalCommission { get; set; }
        public DateTime LastActivity { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class MarketSummaryDto
    {
        public int TotalStocks { get; set; }
        public int ActiveStocks { get; set; }
        public decimal TotalMarketCap { get; set; }
        public decimal TotalVolume { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public decimal TotalTransactionValue { get; set; }
        public DateTime ReportDate { get; set; }
    }
}