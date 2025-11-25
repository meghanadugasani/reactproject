using Sharemarketsimulation.Services;

namespace Sharemarketsimulation.Extensions
{
    public static class CommissionServiceExtensions
    {
        public static void UpdateCommissionRate(this CommissionService service, string stockSymbol, decimal rate)
        {
            // For now, use the same method as SetCommissionRate
            // This would be implemented differently in the actual service
            service.SetCommissionRate(stockSymbol, rate);
        }
    }
}