using System.ComponentModel.DataAnnotations;

namespace Sharemarketsimulation.DTOs
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterDto
    {
        [Required(ErrorMessage = "First name (string) is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name (string) must be 2-50 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name (string) can only contain letters and spaces")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Last name (string) is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name (string) must be 2-50 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last name (string) can only contain letters and spaces")]
        public string LastName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email (string) is required")]
        [EmailAddress(ErrorMessage = "Email (string) invalid format")]
        [StringLength(255, ErrorMessage = "Email (string) cannot exceed 255 characters")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Phone number (string) is required")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Phone number (string) invalid Indian format (10 digits starting with 6-9)")]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Password (string) is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password (string) must be 8-100 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
            ErrorMessage = "Password (string) must contain at least 1 uppercase, 1 lowercase, 1 digit, and 1 special character")]
        public string Password { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Account mode (string) is required")]
        [RegularExpression(@"^(Virtual|Real)$", ErrorMessage = "Account mode (string) must be 'Virtual' or 'Real'")]
        public string AccountMode { get; set; } = "Virtual";
    }

    public class RegisterStaffDto
    {
        [Required(ErrorMessage = "First name (string) is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name (string) must be 2-50 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name (string) can only contain letters and spaces")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Last name (string) is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name (string) must be 2-50 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last name (string) can only contain letters and spaces")]
        public string LastName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email (string) is required")]
        [EmailAddress(ErrorMessage = "Email (string) invalid format")]
        [StringLength(255, ErrorMessage = "Email (string) cannot exceed 255 characters")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Phone number (string) is required")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Phone number (string) invalid Indian format (10 digits starting with 6-9)")]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Password (string) is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password (string) must be 8-100 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
            ErrorMessage = "Password (string) must contain at least 1 uppercase, 1 lowercase, 1 digit, and 1 special character")]
        public string Password { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Role (string) is required")]
        [RegularExpression(@"^(Admin|Broker)$", ErrorMessage = "Role (string) must be 'Admin' or 'Broker'")]
        public string Role { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserDto User { get; set; } = new();
        public DateTime ExpiresAt { get; set; }
        public string AccountMode { get; set; } = string.Empty;
        public decimal AvailableBalance { get; set; }
        public string BalanceType { get; set; } = string.Empty;
    }
}