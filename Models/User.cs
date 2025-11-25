using System.ComponentModel.DataAnnotations;

namespace Sharemarketsimulation.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string? FirstName { get; set; } 
        
        [Required]
        [StringLength(100)]
        public string? LastName { get; set; }
        
        [Required]
        [EmailAddress]
        public string? Email { get; set; } 
        
        [Required]
        public string? Password { get; set; }
        
        [Phone]
        public string? PhoneNumber { get; set; }
        
        public UserRole Role { get; set; } = UserRole.User;
        
        public KYCStatus KYCStatus { get; set; } = KYCStatus.Pending;
        
        public decimal Balance { get; set; } = 100000; 
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        public bool IsActive { get; set; } = true;
        
       
        public virtual ICollection<Portfolio> Portfolios { get; set; } = new List<Portfolio>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public virtual ICollection<Watchlist> Watchlists { get; set; } = new List<Watchlist>();
        public virtual ICollection<UserDocument> Documents { get; set; } = new List<UserDocument>();
    }

    public enum UserRole
    {
        User = 1,
        Admin = 2,
        Broker = 3
    }

    public enum KYCStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        UnderReview = 4
    }
}