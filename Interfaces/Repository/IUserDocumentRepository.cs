using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Interfaces.Repository
{
    public interface IUserDocumentRepository
    {
        Task<UserDocument?> GetByIdAsync(int id);
        Task<IEnumerable<UserDocument>> GetByUserIdAsync(int userId);
        Task<IEnumerable<UserDocument>> GetByStatusAsync(DocumentStatus status);
        Task<IEnumerable<UserDocument>> GetByDocumentTypeAsync(DocumentType documentType);
        Task<UserDocument> CreateAsync(UserDocument document);
        Task<UserDocument> UpdateAsync(UserDocument document);
        Task<bool> DeleteAsync(int id);
        Task<bool> VerifyDocumentAsync(int documentId, int verifiedBy, DocumentStatus status, string? rejectionReason = null);
        Task<IEnumerable<UserDocument>> GetPendingDocumentsAsync();
    }
}