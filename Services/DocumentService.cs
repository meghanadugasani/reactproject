using Sharemarketsimulation.DTOs;
using Sharemarketsimulation.Interfaces;
using Sharemarketsimulation.Interfaces.Repository;
using Sharemarketsimulation.Interfaces.Services;
using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IUserDocumentRepository _documentRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public DocumentService(IUserDocumentRepository documentRepository, IWebHostEnvironment environment, IConfiguration configuration)
        {
            _documentRepository = documentRepository;
            _environment = environment;
            _configuration = configuration;
        }

        public async Task<DocumentDto> UploadDocumentAsync(int userId, UploadDocumentDto uploadDto)
        {
            var uploadPath = _configuration.GetValue<string>("FileUpload:UploadPath", "wwwroot/uploads");
            var fullUploadPath = Path.Combine(_environment.ContentRootPath, uploadPath);
            
            if (!Directory.Exists(fullUploadPath))
                Directory.CreateDirectory(fullUploadPath);

            var fileName = $"{userId}_{uploadDto.DocumentType}_{DateTime.UtcNow:yyyyMMddHHmmss}_{uploadDto.File.FileName}";
            var filePath = Path.Combine(fullUploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await uploadDto.File.CopyToAsync(stream);
            }

            var document = new UserDocument
            {
                UserId = userId,
                DocumentType = uploadDto.DocumentType,
                FileName = uploadDto.File.FileName,
                FilePath = filePath,
                FileSize = FormatFileSize(uploadDto.File.Length),
                ContentType = uploadDto.File.ContentType,
                Status = DocumentStatus.Pending,
                UploadedAt = DateTime.UtcNow
            };

            var createdDocument = await _documentRepository.CreateAsync(document);
            return MapToDto(createdDocument);
        }

        public async Task<IEnumerable<DocumentDto>> GetUserDocumentsAsync(int userId)
        {
            var documents = await _documentRepository.GetByUserIdAsync(userId);
            return documents.Select(MapToDto);
        }

        public async Task<IEnumerable<DocumentDto>> GetPendingDocumentsAsync()
        {
            var documents = await _documentRepository.GetPendingDocumentsAsync();
            return documents.Select(MapToDto);
        }

        public async Task<DocumentDto> VerifyDocumentAsync(int documentId, int verifiedBy, VerifyDocumentDto verifyDto)
        {
            var success = await _documentRepository.VerifyDocumentAsync(documentId, verifiedBy, verifyDto.Status, verifyDto.RejectionReason);
            if (!success) throw new ArgumentException("Document not found");

            var document = await _documentRepository.GetByIdAsync(documentId);
            return MapToDto(document!);
        }

        public async Task<bool> DeleteDocumentAsync(int documentId, int userId)
        {
            var document = await _documentRepository.GetByIdAsync(documentId);
            if (document == null || document.UserId != userId) return false;

            // Delete physical file
            if (File.Exists(document.FilePath))
                File.Delete(document.FilePath);

            return await _documentRepository.DeleteAsync(documentId);
        }

        public async Task<string> GetDocumentPathAsync(int documentId)
        {
            var document = await _documentRepository.GetByIdAsync(documentId);
            if (document == null) throw new ArgumentException("Document not found");
            return document.FilePath;
        }

        public async Task<bool> ValidateFileAsync(IFormFile file)
        {
            var maxFileSize = _configuration.GetValue<long>("FileUpload:MaxFileSize", 5242880); // 5MB
            var allowedExtensions = _configuration.GetSection("FileUpload:AllowedExtensions").Get<string[]>() 
                ?? new[] { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };

            if (file.Length > maxFileSize)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }

        private static DocumentDto MapToDto(UserDocument document)
        {
            return new DocumentDto
            {
                Id = document.Id,
                UserId = document.UserId,
                DocumentType = document.DocumentType,
                FileName = document.FileName,
                FileSize = document.FileSize,
                ContentType = document.ContentType,
                Status = document.Status,
                UploadedAt = document.UploadedAt,
                VerifiedAt = document.VerifiedAt,
                VerifiedByName = document.VerifiedByUser != null ? $"{document.VerifiedByUser.FirstName} {document.VerifiedByUser.LastName}" : null,
                RejectionReason = document.RejectionReason
            };
        }

        private static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}