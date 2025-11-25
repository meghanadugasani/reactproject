using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sharemarketsimulation.DTOs;
using Sharemarketsimulation.Interfaces;
using Sharemarketsimulation.Interfaces.Services;
using Sharemarketsimulation.Extensions;

namespace Sharemarketsimulation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPortfolioService _portfolioService;
        private readonly ITransactionService _transactionService;

        public WalletController(IUserService userService, IPortfolioService portfolioService, ITransactionService transactionService)
        {
            _userService = userService;
            _portfolioService = portfolioService;
            _transactionService = transactionService;
        }

        [HttpGet("balance/{userId}")]
        [Authorize(Roles = "User,Admin")] 
        public async Task<IActionResult> GetBalance(int userId)
        {
            var balance = await _userService.GetUserBalanceAsync(userId);
            var portfolioValue = await _portfolioService.GetPortfolioValueAsync(userId);
            var totalPnL = await _portfolioService.GetTotalProfitLossAsync(userId);

            var walletBalance = new WalletBalanceDto
            {
                AvailableBalance = balance,
                InvestedAmount = portfolioValue - totalPnL,
                TotalValue = balance + portfolioValue,
                ProfitLoss = totalPnL,
                ProfitLossPercent = portfolioValue > 0 ? (totalPnL / (portfolioValue - totalPnL)) * 100 : 0,
                LastUpdated = DateTime.UtcNow
            };

            return Ok(walletBalance);
        }

        [HttpPost("wallet-data")]
        public async Task<IActionResult> GetWalletData([FromBody] WalletDataRequest request)
        {
            Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            
            var balance = await _userService.GetUserBalanceAsync(request.UserId);
            var userTransactions = await _transactionService.GetWalletTransactionsAsync(request.UserId);
            
            return Ok(new { 
                balance = new { availableBalance = balance },
                transactions = userTransactions,
                requestName = request.RequestName 
            });
        }

        public class WalletDataRequest
        {
            public int UserId { get; set; }
            public string RequestName { get; set; } = "WalletData";
        }

        [HttpPost("addfunds")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> AddFunds([FromQuery] int userId, [FromBody] AddFundsDto addFundsDto)
        {
            try
            {
                if (addFundsDto.Amount <= 0)
                    return BadRequest(new { message = "Amount must be greater than zero" });
                    
                var success = await _userService.AddFundsAsync(userId, addFundsDto.Amount);
                if (!success)
                    return BadRequest(new { message = "Failed to add funds" });

                // Track deposit transaction
                await _transactionService.CreateWalletTransactionAsync(userId, "Deposit", addFundsDto.Amount, 
                    addFundsDto.PaymentMethod ?? "Manual", addFundsDto.TransactionReference ?? $"DEP_{Guid.NewGuid().ToString()[..8]}");

                return Ok(new { message = "Funds added successfully", amount = addFundsDto.Amount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("withdraw")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> WithdrawFunds([FromQuery] int userId, [FromBody] WithdrawFundsDto withdrawDto)
        {
            // Get account mode from JWT token
            var accountMode = User.FindFirst("AccountMode")?.Value;
            if (string.IsNullOrEmpty(accountMode))
                return BadRequest(new { message = "Account mode not found in token" });
            
            // Block virtual users from withdrawing
            if (accountMode == "Virtual")
                return BadRequest(new { message = "You are a virtual user, unable to get amount. Virtual money cannot be withdrawn." });
            
            var success = await _userService.WithdrawFundsAsync(userId, withdrawDto.Amount);
            if (!success)
                return BadRequest(new { message = "Insufficient balance or withdrawal failed" });

            // Track withdrawal transaction
            await _transactionService.CreateWalletTransactionAsync(userId, "Withdrawal", withdrawDto.Amount, 
                withdrawDto.BankAccount ?? "Bank Transfer", $"WTH_{Guid.NewGuid().ToString()[..8]}");

            return Ok(new { message = "Withdrawal successful", amount = withdrawDto.Amount });
        }

        [HttpGet("transactions/{userId}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetFundTransactions(int userId)
        {
            // Return only wallet fund transactions for this user
            var userTransactions = await _transactionService.GetWalletTransactionsAsync(userId);
            
            return Ok(userTransactions);
        }

        [HttpPost("bank-account")]
        [Authorize(Roles = "User")]
        public IActionResult AddBankAccount(BankAccountDto bankAccountDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(new { message = "Validation failed", errors });
                }
                
                if (string.IsNullOrEmpty(bankAccountDto.IFSC) || !IsValidIFSC(bankAccountDto.IFSC))
                    return BadRequest(new { message = "IFSC code format is invalid" });
                    
                if (string.IsNullOrEmpty(bankAccountDto.AccountNumber) || !IsValidAccountNumber(bankAccountDto.AccountNumber))
                    return BadRequest(new { message = "Account number format is invalid" });
                
                var response = new
                {
                    message = "Bank account added and verified successfully",
                    accountNumber = "****" + (bankAccountDto.AccountNumber?.Length > 4 ? bankAccountDto.AccountNumber.Substring(bankAccountDto.AccountNumber.Length - 4) : "****"),
                    bankName = bankAccountDto.BankName,
                    ifsc = bankAccountDto.IFSC,
                    accountHolderName = bankAccountDto.AccountHolderName,
                    accountType = bankAccountDto.AccountType,
                    status = "Verified",
                    verificationId = Guid.NewGuid().ToString(),
                    verifiedAt = DateTime.UtcNow
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("bank-accounts/{userId}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetBankAccounts(int userId)
        {
            try
            {
                var accounts = await _userService.GetUserBankAccountsAsync(userId);
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("deposit")]
        [Authorize(Roles = "User")]
        
        public async Task<IActionResult> DepositMoney([FromBody] DepositRequestDto depositDto)

        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { message = "Validation failed", errors, modelState = ModelState });
            }
            
            var accountMode = User.FindFirst("AccountMode")?.Value;
            if (string.IsNullOrEmpty(accountMode))
                return BadRequest(new { message = "Account mode not found in token" });
            if (accountMode == "Virtual")
                return BadRequest(new { message = "Cannot deposit real money in Virtual mode. Virtual users get ₹10,000 practice money automatically." });
                
            if (depositDto.PaymentMethod == "UPI" && string.IsNullOrEmpty(depositDto.UPIId))
                return BadRequest(new { message = "UPI ID is required for UPI payments" });
                
            if (depositDto.PaymentMethod == "NetBanking" && string.IsNullOrEmpty(depositDto.BankAccountId))
                return BadRequest(new { message = "Bank account is required for Net Banking" });
            
            var transactionId = Guid.NewGuid().ToString();
            var paymentResponse = new
            {
                transactionId = transactionId,
                amount = depositDto.Amount,
                status = "Success",
                paymentMethod = depositDto.PaymentMethod,
                timestamp = DateTime.UtcNow,
                message = "Money deposited successfully"
            };

            var success = await _userService.AddFundsAsync(depositDto.UserId, depositDto.Amount);
            if (!success)
                return BadRequest(new { message = "Failed to update balance" });

            // Track deposit transaction
            await _transactionService.CreateWalletTransactionAsync(depositDto.UserId, "Deposit", depositDto.Amount, 
                depositDto.PaymentMethod, transactionId);

            return Ok(paymentResponse);
        }

        [HttpPost("withdrawToBank")]

        [Authorize(Roles = "User")]
        public async Task<IActionResult> WithdrawToBank([FromBody] WithdrawToBankDto withdrawDto)

        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { message = "Validation failed", errors, modelState = ModelState });
            }
            
            var user = await _userService.GetUserProfileAsync(withdrawDto.UserId);
            if (user == null)
                return NotFound(new { message = "User not found" });
            
            if (user.AccountMode == "Virtual")
                return BadRequest(new { message = "You are a virtual user, unable to get amount. In wallet they cannot withdraw any money." });
            
            decimal withdrawalFee = Math.Max(withdrawDto.Amount * 0.005m, 20); 
            decimal totalDeduction = withdrawDto.Amount + withdrawalFee;
            
            var balance = await _userService.GetUserBalanceAsync(withdrawDto.UserId);
            if (balance < totalDeduction)
            {
                return BadRequest(new { message = $"Insufficient balance. Required: ₹{totalDeduction} (Amount: ₹{withdrawDto.Amount} + Fee: ₹{withdrawalFee})" });
            }

            var transferResponse = new
            {
                transactionId = Guid.NewGuid().ToString(),
                accountMode = "Real Money",
                requestedAmount = withdrawDto.Amount,
                withdrawalFee = withdrawalFee,
                totalDeducted = totalDeduction,
                netAmountToBank = withdrawDto.Amount,
                bankAccount = withdrawDto.BankAccountId,
                status = "Processing",
                estimatedTime = "2-3 business days",
                timestamp = DateTime.UtcNow,
                message = $"Real money withdrawal: ₹{withdrawDto.Amount} to bank (Fee: ₹{withdrawalFee})"
            };

            var success = await _userService.WithdrawFundsAsync(withdrawDto.UserId, totalDeduction);
            if (!success)
                return BadRequest(new { message = "Withdrawal failed" });

            // Track withdrawal transaction
            await _transactionService.CreateWalletTransactionAsync(withdrawDto.UserId, "Withdrawal", withdrawDto.Amount, 
                "Bank Transfer", transferResponse.transactionId, "Processing");
                
            Console.WriteLine($"✅ REAL MONEY: Withdrawal fee ₹{withdrawalFee} earned by broker from user {withdrawDto.UserId}");

            return Ok(transferResponse);
        }

        [HttpGet("payment-methods")]
        public IActionResult GetPaymentMethods()
        {
            var methods = new[]
            {
                new { id = "upi", name = "UPI", description = "Pay using UPI apps like GPay, PhonePe", processingTime = "Instant" },
                new { id = "netbanking", name = "Net Banking", description = "Direct bank transfer", processingTime = "Instant" },
                new { id = "debitcard", name = "Debit Card", description = "Pay using debit card", processingTime = "Instant" },
                new { id = "creditcard", name = "Credit Card", description = "Pay using credit card", processingTime = "Instant" }
            };
            return Ok(methods);
        }
        
        
        private static bool IsValidIFSC(string ifsc)
        {
          
            return System.Text.RegularExpressions.Regex.IsMatch(ifsc, @"^[A-Z]{4}0[A-Z0-9]{6}$");
        }
        
        private static bool IsValidAccountNumber(string accountNumber)
        {
            
            return System.Text.RegularExpressions.Regex.IsMatch(accountNumber, @"^[0-9]{9,18}$");
        }
        
        private static bool IsValidUPI(string upiId)
        {
           
            return System.Text.RegularExpressions.Regex.IsMatch(upiId, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+$");
        }
    }
}