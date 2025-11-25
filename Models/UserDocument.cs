using System.ComponentModel.DataAnnotations;

namespace Sharemarketsimulation.Models
{
    public class UserDocument
    {
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public DocumentType DocumentType { get; set; }
        
        [Required]
        [StringLength(500)]
        public string? FileName { get; set; }
        
        [Required]
        [StringLength(1000)]
        public string? FilePath { get; set; } 
        
        [StringLength(100)]
        public string? FileSize { get; set; }
        
        [StringLength(50)]
        public string? ContentType { get; set; }
        
        public DocumentStatus Status { get; set; } = DocumentStatus.Pending;
        
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? VerifiedAt { get; set; }
        
        public int? VerifiedBy { get; set; }
        
        public string? RejectionReason { get; set; }
        
        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual User? VerifiedByUser { get; set; }
    }

    public enum DocumentType
    {
        PAN = 1,
        Aadhaar = 2,
        Passport = 3,
        DrivingLicense = 4,
        BankStatement = 5,
        IncomeProof = 6,
        AddressProof = 7
    }

    public enum DocumentStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        UnderReview = 4
    }
}