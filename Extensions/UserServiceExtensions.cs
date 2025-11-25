using Sharemarketsimulation.Interfaces.Services;

namespace Sharemarketsimulation.Extensions
{
    public static class UserServiceExtensions
    {
        public static async Task<object[]> GetUserBankAccountsAsync(this IUserService service, int userId)
        {
            // This would be implemented in the actual service
            // For now, return sample data to make the controller work
            await Task.CompletedTask;
            return new object[]
            {
                new { id = 1, bankName = "HDFC Bank", accountNumber = "****1234", ifsc = "HDFC0001234", status = "Verified" },
                new { id = 2, bankName = "SBI", accountNumber = "****5678", ifsc = "SBIN0001234", status = "Pending" }
            };
        }
    }
}