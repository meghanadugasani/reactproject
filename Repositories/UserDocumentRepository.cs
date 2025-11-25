using Microsoft.EntityFrameworkCore;
using Sharemarketsimulation.Data;
using Sharemarketsimulation.Interfaces;
using Sharemarketsimulation.Interfaces.Repository;
using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Repositories
{
    public class UserDocumentRepository : IUserDocumentRepository
    {
        private readonly StockMarketDbContext _context;

        public UserDocumentRepository(StockMarketDbContext context)
        {
            _context = context;
        }

        public async Task<UserDocument?> GetByIdAsync(int id)
        {
            return await _context.UserDocuments
                .Include(d => d.User)
                .Include(d => d.VerifiedByUser)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<UserDocument>> GetByUserIdAsync(int userId)
        {
            return await _context.UserDocuments
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.UploadedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserDocument>> GetByStatusAsync(DocumentStatus status)
        {
            return await _context.UserDocuments
                .Include(d => d.User)
                .Where(d => d.Status == status)
                .OrderBy(d => d.UploadedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserDocument>> GetByDocumentTypeAsync(DocumentType documentType)
        {
            return await _context.UserDocuments
                .Include(d => d.User)
                .Where(d => d.DocumentType == documentType)
                .ToListAsync();
        }

        public async Task<UserDocument> CreateAsync(UserDocument document)
        {
            _context.UserDocuments.Add(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<UserDocument> UpdateAsync(UserDocument document)
        {
            _context.UserDocuments.Update(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var document = await _context.UserDocuments.FindAsync(id);
            if (document == null) return false;

            _context.UserDocuments.Remove(document);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> VerifyDocumentAsync(int documentId, int verifiedBy, DocumentStatus status, string? rejectionReason = null)
        {
            var document = await _context.UserDocuments.FindAsync(documentId);
            if (document == null) return false;

            document.Status = status;
            document.VerifiedBy = verifiedBy;
            document.VerifiedAt = DateTime.UtcNow;
            document.RejectionReason = rejectionReason;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<UserDocument>> GetPendingDocumentsAsync()
        {
            return await GetByStatusAsync(DocumentStatus.Pending);
        }
    }
}