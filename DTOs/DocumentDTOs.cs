using System.ComponentModel.DataAnnotations;
using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.DTOs
{
    public class UploadDocumentDto
    {
        [Required]
        public DocumentType DocumentType { get; set; }
        
        [Required]
        public IFormFile File { get; set; } = null!;
    }

    public class DocumentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DocumentType DocumentType { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string? FileSize { get; set; }
        public string? ContentType { get; set; }
        public DocumentStatus Status { get; set; }
        public DateTime UploadedAt { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string? VerifiedByName { get; set; }
        public string? RejectionReason { get; set; }
    }

    public class VerifyDocumentDto
    {
        [Required]
        public DocumentStatus Status { get; set; }
        
        public string? RejectionReason { get; set; }
    }

    public class DocumentFilterDto
    {
        public int? UserId { get; set; }
        public DocumentType? DocumentType { get; set; }
        public DocumentStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}