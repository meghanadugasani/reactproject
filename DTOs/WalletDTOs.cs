using System.ComponentModel.DataAnnotations;

namespace Sharemarketsimulation.DTOs
{
    public class AddFundsDto
    {
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
        
        public string? PaymentMethod { get; set; }
        
        public string? TransactionReference { get; set; }
    }

    public class WithdrawFundsDto
    {
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
        
        public string? BankAccount { get; set; }
        
        public string? Reason { get; set; }
    }

    public class WalletBalanceDto
    {
        public decimal AvailableBalance { get; set; }
        public decimal InvestedAmount { get; set; }
        public decimal TotalValue { get; set; }
        public decimal ProfitLoss { get; set; }
        public decimal ProfitLossPercent { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class FundTransactionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? Reference { get; set; }
    }

    public class BankAccountDto
    {
        [Required(ErrorMessage = "Account number is required")]
        [RegularExpression(@"^[0-9]{9,18}$", ErrorMessage = "Account number must be 9-18 digits")]
        public string AccountNumber { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Bank name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Bank name must be 2-100 characters")]
        public string BankName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "IFSC code is required")]
        [RegularExpression(@"^[A-Z]{4}0[A-Z0-9]{6}$", ErrorMessage = "Invalid IFSC format (e.g., HDFC0001234)")]
        public string IFSC { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Account holder name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Account holder name must be 2-100 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Account holder name can only contain letters and spaces")]
        public string AccountHolderName { get; set; } = string.Empty;
        
        [RegularExpression(@"^(Savings|Current)$", ErrorMessage = "Account type must be 'Savings' or 'Current'")]
        public string AccountType { get; set; } = "Savings";
    }

    public class DepositRequestDto
    {
        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }
        
        [Required(ErrorMessage = "Amount is required")]
        [Range(1, 1000000, ErrorMessage = "Amount must be between ₹1 and ₹10,00,000")]
        public decimal Amount { get; set; }
        
        [Required(ErrorMessage = "Payment method is required")]
        [RegularExpression(@"^(UPI|NetBanking|DebitCard|CreditCard)$", ErrorMessage = "Invalid payment method")]
        public string PaymentMethod { get; set; } = string.Empty;
        
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+$", ErrorMessage = "Invalid UPI ID format (e.g., user@paytm)")]
        public string? UPIId { get; set; }
        
        public string? BankAccountId { get; set; }
    }

    public class WithdrawToBankDto
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        [Range(1, double.MaxValue)]
        public decimal Amount { get; set; }
        
        [Required]
        public string BankAccountId { get; set; } = string.Empty;
        
        public string Purpose { get; set; } = "Withdrawal";
    }
}