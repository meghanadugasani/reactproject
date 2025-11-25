using Sharemarketsimulation.DTOs;
using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Interfaces.Services
{
    public interface ITransactionService
    {
        Task<TransactionDto> CreateTransactionAsync(int userId, int stockId, int? orderId, TransactionType type, int quantity, decimal price);
        Task<IEnumerable<TransactionDto>> GetUserTransactionsAsync(int userId, TransactionFilterDto? filter = null);
        Task<TransactionSummaryDto> GetTransactionSummaryAsync(int userId);
        Task<IEnumerable<TransactionDto>> GetTransactionHistoryAsync(int userId, int page, int pageSize);
        Task<decimal> CalculateCommissionAsync(decimal amount);
        Task<decimal> CalculateTaxAsync(decimal amount);
        Task<decimal> GetTotalFeesAsync(int userId);
        Task<IEnumerable<TransactionDto>> GetTransactionsByDateAsync(DateTime date);
        Task<IEnumerable<TransactionDto>> GetTransactionsByStockAsync(int stockId);
        Task<decimal> GetTotalVolumeAsync(int userId);
        Task<decimal> GetAverageTransactionSizeAsync(int userId);
        Task<int> GetTransactionCountAsync(int userId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<TransactionSummaryDto> GetMonthlyReportAsync(int userId, int year, int month);
        Task<TransactionSummaryDto> GetYearlyReportAsync(int userId, int year);
        Task<IEnumerable<TransactionDto>> GetLargestTransactionsAsync(int userId, int count = 10);
    }
}