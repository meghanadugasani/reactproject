using System.ComponentModel.DataAnnotations;
using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.DTOs
{
    public class CreateOrderDto
    {
        [Required]
        public int StockId { get; set; }
        
        [Required]
        public OrderType OrderType { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        
        public decimal? StopLoss { get; set; }
        
        public decimal? Target { get; set; }
        
        public DateTime? ExpiresAt { get; set; }
        
        public string? Notes { get; set; }
    }

    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int StockId { get; set; }
        public string StockSymbol { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public OrderType OrderType { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal? StopLoss { get; set; }
        public decimal? Target { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExecutedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? Notes { get; set; }
        public string AccountMode { get; set; } = string.Empty;
    }

    public class UpdateOrderDto
    {
        [Range(1, int.MaxValue)]
        public int? Quantity { get; set; }
        
        [Range(0.01, double.MaxValue)]
        public decimal? Price { get; set; }
        
        [Range(0.01, double.MaxValue)]
        public decimal? StopLoss { get; set; }
        
        [Range(0.01, double.MaxValue)]
        public decimal? Target { get; set; }
        
        public DateTime? ExpiresAt { get; set; }
        
        public string? Notes { get; set; }
    }
}