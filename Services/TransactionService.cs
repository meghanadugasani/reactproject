using Microsoft.Extensions.Configuration;
using Sharemarketsimulation.DTOs;
using Sharemarketsimulation.Interfaces;
using Sharemarketsimulation.Interfaces.Repository;
using Sharemarketsimulation.Interfaces.Services;
using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IConfiguration _configuration;
        private readonly BrokerCommissionService _brokerCommissionService;

        public TransactionService(ITransactionRepository transactionRepository, IConfiguration configuration, BrokerCommissionService brokerCommissionService)
        {
            _transactionRepository = transactionRepository;
            _configuration = configuration;
            _brokerCommissionService = brokerCommissionService;
        }

        public async Task<TransactionDto> CreateTransactionAsync(int userId, int stockId, int? orderId, TransactionType type, int quantity, decimal price)
        {
            var totalAmount = quantity * price;
            var commission = await CalculateCommissionAsync(totalAmount);
            var tax = await CalculateTaxAsync(totalAmount);
            var netAmount = type == TransactionType.Buy ? 
                totalAmount + commission + tax : 
                totalAmount - commission - tax;

            var transaction = new Transaction
            {
                UserId = userId,
                StockId = stockId,
                OrderId = orderId,
                TransactionType = type,
                Quantity = quantity,
                Price = price,
                TotalAmount = totalAmount,
                Commission = commission,
                Tax = tax,
                NetAmount = netAmount,
                TransactionDate = DateTime.UtcNow
            };

            var createdTransaction = await _transactionRepository.CreateAsync(transaction);
            
          
            await PayCommissionToBrokerAsync(commission, userId);
            
            return MapToDto(createdTransaction);
        }
      
        private async Task PayCommissionToBrokerAsync(decimal commission, int userId)
        {
            
            var user = await GetUserAsync(userId);
            var accountMode = GetAccountModeFromContext(); 
            
            if (accountMode == "Virtual")
            {
               
                return;
            }
            
            var brokerId = await GetAssignedBrokerAsync(userId);
            if (brokerId == null)
            {
               
                brokerId = await GetDefaultBrokerAsync();
            }
            
            //  Add REAL commission to broker's balance
            if (brokerId.HasValue)
            {
                await AddCommissionToBrokerBalanceAsync(brokerId.Value, commission);
                
                await LogCommissionPaymentAsync(brokerId.Value, userId, commission);
            }
        }
        
        private string GetAccountModeFromContext()
        {
            
            return "Real";
        }
        
        private async Task<User?> GetUserAsync(int userId)
        {
          
            return null; 
        }
        
        private async Task<int?> GetAssignedBrokerAsync(int userId)
        {
            
            return 1; 
        }
        
        private async Task<int?> GetDefaultBrokerAsync()
        {
            
            return 1;
        }
        
        private async Task AddCommissionToBrokerBalanceAsync(int brokerId, decimal commission)
        {
            
            var accountMode = GetAccountModeFromContext();
            var success = await _brokerCommissionService.PayCommissionToBrokerAsync(brokerId, commission, accountMode);
            
            if (success)
            {
                Console.WriteLine($"REAL COMMISSION PAID: Broker {brokerId} received ₹{commission}");
            }
            else
            {
                Console.WriteLine($"COMMISSION NOT PAID: Virtual trade or invalid broker {brokerId}");
            }
        }
        
        private async Task LogCommissionPaymentAsync(int brokerId, int userId, decimal commission)
        {
           
            Console.WriteLine($"Commission Log: Broker {brokerId} earned ₹{commission} from User {userId} trade");
        }

        public async Task<IEnumerable<TransactionDto>> GetUserTransactionsAsync(int userId, TransactionFilterDto? filter = null)
        {
            var transactions = await _transactionRepository.GetByUserIdAsync(userId);
            
            if (filter != null)
            {
                if (filter.StockId.HasValue)
                    transactions = transactions.Where(t => t.StockId == filter.StockId.Value);
                
                if (filter.TransactionType.HasValue)
                    transactions = transactions.Where(t => t.TransactionType == filter.TransactionType.Value);
                
                if (filter.FromDate.HasValue)
                    transactions = transactions.Where(t => t.TransactionDate >= filter.FromDate.Value);
                
                if (filter.ToDate.HasValue)
                    transactions = transactions.Where(t => t.TransactionDate <= filter.ToDate.Value);
            }

            return transactions.Select(MapToDto);
        }

        public async Task<TransactionSummaryDto> GetTransactionSummaryAsync(int userId)
        {
            var transactions = await _transactionRepository.GetByUserIdAsync(userId);
            
            var buyTransactions = transactions.Where(t => t.TransactionType == TransactionType.Buy);
            var sellTransactions = transactions.Where(t => t.TransactionType == TransactionType.Sell);

            return new TransactionSummaryDto
            {
                TotalBuyAmount = buyTransactions.Sum(t => t.TotalAmount),
                TotalSellAmount = sellTransactions.Sum(t => t.TotalAmount),
                TotalCommission = transactions.Sum(t => t.Commission),
                TotalTax = transactions.Sum(t => t.Tax),
                TotalTransactions = transactions.Count(),
                RecentTransactions = transactions.Take(10).Select(MapToDto).ToList()
            };
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionHistoryAsync(int userId, int page, int pageSize)
        {
            var transactions = await _transactionRepository.GetUserTransactionHistoryAsync(userId, page, pageSize);
            return transactions.Select(MapToDto);
        }

        public async Task<decimal> CalculateCommissionAsync(decimal amount)
        {
            var commissionRate = _configuration.GetValue<decimal>("StockSettings:CommissionRate", 0.001m);
            return amount * commissionRate;
        }

        public async Task<decimal> CalculateTaxAsync(decimal amount)
        {
            var taxRate = _configuration.GetValue<decimal>("StockSettings:TaxRate", 0.0005m);
            return amount * taxRate;
        }

        public async Task<decimal> GetTotalFeesAsync(int userId)
        {
            var commission = await _transactionRepository.GetTotalCommissionAsync(userId);
            var tax = await _transactionRepository.GetTotalTaxAsync(userId);
            return commission + tax;
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionsByDateAsync(DateTime date)
        {
            var transactions = await _transactionRepository.GetByDateRangeAsync(date.Date, date.Date.AddDays(1));
            return transactions.Select(MapToDto);
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionsByStockAsync(int stockId)
        {
            var transactions = await _transactionRepository.GetByStockIdAsync(stockId);
            return transactions.Select(MapToDto);
        }

        public async Task<decimal> GetTotalVolumeAsync(int userId)
        {
            var transactions = await _transactionRepository.GetByUserIdAsync(userId);
            return transactions.Sum(t => t.TotalAmount);
        }

        public async Task<decimal> GetAverageTransactionSizeAsync(int userId)
        {
            var transactions = await _transactionRepository.GetByUserIdAsync(userId);
            return transactions.Any() ? transactions.Average(t => t.TotalAmount) : 0;
        }

        public async Task<int> GetTransactionCountAsync(int userId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var transactions = await _transactionRepository.GetByUserIdAsync(userId);
            if (fromDate.HasValue)
                transactions = transactions.Where(t => t.TransactionDate >= fromDate.Value);
            if (toDate.HasValue)
                transactions = transactions.Where(t => t.TransactionDate <= toDate.Value);
            return transactions.Count();
        }

        public async Task<TransactionSummaryDto> GetMonthlyReportAsync(int userId, int year, int month)
        {
            var fromDate = new DateTime(year, month, 1);
            var toDate = fromDate.AddMonths(1).AddDays(-1);
            var transactions = await _transactionRepository.GetByDateRangeAsync(fromDate, toDate);
            var userTransactions = transactions.Where(t => t.UserId == userId);
            
            return new TransactionSummaryDto
            {
                TotalBuyAmount = userTransactions.Where(t => t.TransactionType == TransactionType.Buy).Sum(t => t.TotalAmount),
                TotalSellAmount = userTransactions.Where(t => t.TransactionType == TransactionType.Sell).Sum(t => t.TotalAmount),
                TotalCommission = userTransactions.Sum(t => t.Commission),
                TotalTax = userTransactions.Sum(t => t.Tax),
                TotalTransactions = userTransactions.Count()
            };
        }

        public async Task<TransactionSummaryDto> GetYearlyReportAsync(int userId, int year)
        {
            var fromDate = new DateTime(year, 1, 1);
            var toDate = new DateTime(year, 12, 31);
            var transactions = await _transactionRepository.GetByDateRangeAsync(fromDate, toDate);
            var userTransactions = transactions.Where(t => t.UserId == userId);
            
            return new TransactionSummaryDto
            {
                TotalBuyAmount = userTransactions.Where(t => t.TransactionType == TransactionType.Buy).Sum(t => t.TotalAmount),
                TotalSellAmount = userTransactions.Where(t => t.TransactionType == TransactionType.Sell).Sum(t => t.TotalAmount),
                TotalCommission = userTransactions.Sum(t => t.Commission),
                TotalTax = userTransactions.Sum(t => t.Tax),
                TotalTransactions = userTransactions.Count()
            };
        }

        public async Task<IEnumerable<TransactionDto>> GetLargestTransactionsAsync(int userId, int count = 10)
        {
            var transactions = await _transactionRepository.GetByUserIdAsync(userId);
            return transactions.OrderByDescending(t => t.TotalAmount).Take(count).Select(MapToDto);
        }

        private static TransactionDto MapToDto(Transaction transaction)
        {
            return new TransactionDto
            {
                Id = transaction.Id,
                UserId = transaction.UserId,
                StockId = transaction.StockId,
                StockSymbol = transaction.Stock?.Symbol ?? "",
                CompanyName = transaction.Stock?.CompanyName ?? "",
                OrderId = transaction.OrderId,
                TransactionType = transaction.TransactionType,
                Quantity = transaction.Quantity,
                Price = transaction.Price,
                TotalAmount = transaction.TotalAmount,
                Commission = transaction.Commission,
                Tax = transaction.Tax,
                NetAmount = transaction.NetAmount,
                TransactionDate = transaction.TransactionDate,
                Notes = transaction.Notes
            };
        }
    }
}