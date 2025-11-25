using Sharemarketsimulation.Interfaces.Services;

namespace Sharemarketsimulation.Extensions
{
    public static class TransactionServiceExtensions
    {
        public static async Task CreateWalletTransactionAsync(this ITransactionService service, 
            int userId, string type, decimal amount, string paymentMethod, string reference, string status = "Completed")
        {
            // This would be implemented in the actual service
            // For now, this is a placeholder to make the controller compile
            await Task.CompletedTask;
        }

        public static async Task<object[]> GetWalletTransactionsAsync(this ITransactionService service, int userId)
        {
            // This would be implemented in the actual service
            // For now, return empty array to make the controller compile
            return Array.Empty<object>();
        }
    }
}