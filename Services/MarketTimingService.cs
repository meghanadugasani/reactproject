using Microsoft.Extensions.Configuration;

namespace Sharemarketsimulation.Services
{
    public class MarketTimingService
    {
        private readonly IConfiguration _configuration;
        private static bool _isManuallyOverridden = false;
        private static bool _manualMarketState = false;

        public MarketTimingService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

      
        public bool IsMarketOpen()
        {
           
            if (_isManuallyOverridden)
            {
                return _manualMarketState;
            }
            
           
            var now = DateTime.Now.TimeOfDay;
            
        
            if (!TimeSpan.TryParseExact(_configuration["StockSettings:MarketOpenTime"] ?? "09:30", @"hh\:mm", null, out var openTime))
                openTime = new TimeSpan(9, 30, 0);
                
            if (!TimeSpan.TryParseExact(_configuration["StockSettings:MarketCloseTime"] ?? "16:00", @"hh\:mm", null, out var closeTime))
                closeTime = new TimeSpan(16, 0, 0);

           
            var isWithinHours = now >= openTime && now <= closeTime;
            
            
            var isWeekday = DateTime.Now.DayOfWeek >= DayOfWeek.Monday && DateTime.Now.DayOfWeek <= DayOfWeek.Friday;

            return isWithinHours && isWeekday;
        }

     
        public object GetMarketStatus()
        {
            var isOpen = IsMarketOpen();
            var openTime = _configuration["StockSettings:MarketOpenTime"] ?? "09:30";
            var closeTime = _configuration["StockSettings:MarketCloseTime"] ?? "16:00";

            return new
            {
                isOpen,
                status = isOpen ? "OPEN" : "CLOSED",
                openTime,
                closeTime,
                currentTime = DateTime.Now.ToString("HH:mm"),
                tradingDays = "Monday to Friday",
                message = isOpen ? 
                    "Market is open for trading" : 
                    "Market is closed. Trading will resume during market hours."
            };
        }

 
        public (bool allowed, string message) ValidateTrading(string accountMode)
        {
    
            if (accountMode == "Real")
            {
                return (true, "Real Money trading allowed 24/7");
            }
            
      
            if (!IsMarketOpen())
            {
                return (false, "Virtual Money trading is not allowed. Market is closed. Real Money users can trade 24/7.");
            }

            return (true, "Virtual Money trading allowed during market hours");
        }
        
   
        public void OpenMarketManually()
        {
            _isManuallyOverridden = true;
            _manualMarketState = true;
        }
        
    
        public void CloseMarketManually()
        {
            _isManuallyOverridden = true;
            _manualMarketState = false;
        }
        
    
        public void ResetToAutomatic()
        {
            _isManuallyOverridden = false;
        }
    }
}