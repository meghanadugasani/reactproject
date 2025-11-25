using Sharemarketsimulation.DTOs;

namespace Sharemarketsimulation.Interfaces.Services
{
    public interface IDocumentService
    {
        Task<DocumentDto> UploadDocumentAsync(int userId, UploadDocumentDto uploadDto);
        Task<IEnumerable<DocumentDto>> GetUserDocumentsAsync(int userId);
        Task<IEnumerable<DocumentDto>> GetPendingDocumentsAsync();
        Task<DocumentDto> VerifyDocumentAsync(int documentId, int verifiedBy, VerifyDocumentDto verifyDto);
        Task<bool> DeleteDocumentAsync(int documentId, int userId);
        Task<string> GetDocumentPathAsync(int documentId);
        Task<bool> ValidateFileAsync(IFormFile file);
    }
}