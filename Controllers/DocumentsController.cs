using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sharemarketsimulation.DTOs;
using Sharemarketsimulation.Interfaces;
using Sharemarketsimulation.Interfaces.Services;

namespace Sharemarketsimulation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpPost("upload")]
        [Authorize(Roles = "User")] 
        public async Task<IActionResult> UploadDocument([FromQuery] int userId, [FromForm] UploadDocumentDto uploadDto)
        {
            try
            {
                if (!await _documentService.ValidateFileAsync(uploadDto.File))
                    return BadRequest(new { message = "Invalid file type or size" });

                var document = await _documentService.UploadDocumentAsync(userId, uploadDto);
                return Ok(new { message = "Document uploaded successfully", document });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserDocuments(int userId)
        {
            var documents = await _documentService.GetUserDocumentsAsync(userId);
            return Ok(documents);
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> GetPendingDocuments()
        {
            var documents = await _documentService.GetPendingDocumentsAsync();
            return Ok(documents);
        }

        [HttpPost("{documentId}/verify")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> VerifyDocument(int documentId, [FromQuery] int verifiedBy, [FromBody] VerifyDocumentDto verifyDto)
        {
            try
            {
                var document = await _documentService.VerifyDocumentAsync(documentId, verifiedBy, verifyDto);
                return Ok(new { message = "Document verification completed", document });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{documentId}")]
        public async Task<IActionResult> DeleteDocument(int documentId, [FromQuery] int userId)
        {
            var success = await _documentService.DeleteDocumentAsync(documentId, userId);
            if (!success)
                return NotFound(new { message = "Document not found" });

            return Ok(new { message = "Document deleted successfully" });
        }

        [HttpGet("{documentId}/download")]
        public async Task<IActionResult> DownloadDocument(int documentId)
        {
            try
            {
                var filePath = await _documentService.GetDocumentPathAsync(documentId);
                if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                    return NotFound(new { message = "File not found" });

                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                var fileName = Path.GetFileName(filePath) ?? "document";
                
                return File(fileBytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}