using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous] 
    public class EnumsController : ControllerBase
    {
        [HttpGet("user-roles")]
        public IActionResult GetUserRoles()
        {
            var roles = Enum.GetValues<UserRole>()
                .Select(r => new { value = r.ToString(), name = r.ToString() })
                .ToList();
            return Ok(roles);
        }

        [HttpGet("kyc-status")]
        public IActionResult GetKYCStatuses()
        {
            var statuses = Enum.GetValues<KYCStatus>()
                .Select(s => new { value = s.ToString(), name = s.ToString() })
                .ToList();
            return Ok(statuses);
        }

        [HttpGet("stock-categories")]
        public IActionResult GetStockCategories()
        {
            var categories = Enum.GetValues<StockCategory>()
                .Select(c => new { value = c.ToString(), name = c.ToString() })
                .ToList();
            return Ok(categories);
        }

        [HttpGet("order-types")]
        public IActionResult GetOrderTypes()
        {
            var types = Enum.GetValues<OrderType>()
                .Select(t => new { value = t.ToString(), name = t.ToString() })
                .ToList();
            return Ok(types);
        }

        [HttpGet("order-status")]
        public IActionResult GetOrderStatuses()
        {
            var statuses = Enum.GetValues<OrderStatus>()
                .Select(s => new { value = s.ToString(), name = s.ToString() })
                .ToList();
            return Ok(statuses);
        }

        [HttpGet("transaction-types")]
        public IActionResult GetTransactionTypes()
        {
            var types = Enum.GetValues<TransactionType>()
                .Select(t => new { value = t.ToString(), name = t.ToString() })
                .ToList();
            return Ok(types);
        }

        [HttpGet("document-types")]
        public IActionResult GetDocumentTypes()
        {
            var types = Enum.GetValues<DocumentType>()
                .Select(t => new { value = t.ToString(), name = t.ToString() })
                .ToList();
            return Ok(types);
        }

        [HttpGet("alert-types")]
        public IActionResult GetAlertTypes()
        {
            var types = Enum.GetValues<AlertType>()
                .Select(t => new { value = t.ToString(), name = t.ToString() })
                .ToList();
            return Ok(types);
        }
    }
}