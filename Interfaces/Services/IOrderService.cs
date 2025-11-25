using Sharemarketsimulation.DTOs;
using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Interfaces.Services
{
    public interface IOrderService
    {
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync(string accountMode = "Virtual");
        Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId, string accountMode = "Virtual");
        Task<OrderDto> PlaceOrderAsync(int userId, CreateOrderDto orderDto, string accountMode = "Virtual");
        Task<OrderDto> UpdateOrderAsync(int orderId, UpdateOrderDto updateDto);
        Task<bool> CancelOrderAsync(int orderId, int userId);
        Task<IEnumerable<OrderDto>> GetPendingOrdersAsync();
        Task<bool> ExecuteOrderAsync(int orderId);
        Task ProcessExpiredOrdersAsync();
        Task<bool> ValidateOrderAsync(int userId, CreateOrderDto orderDto);
        Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(Models.OrderStatus status);
        Task<IEnumerable<OrderDto>> GetOrdersByStockAsync(int stockId);
        Task<bool> ExecuteMarketOrdersAsync();
        Task<decimal> GetOrderValueAsync(int orderId);
        Task<IEnumerable<OrderDto>> GetOrderHistoryAsync(int userId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<bool> ModifyOrderPriceAsync(int orderId, decimal newPrice);
        Task<bool> ModifyOrderQuantityAsync(int orderId, int newQuantity);
    }
}