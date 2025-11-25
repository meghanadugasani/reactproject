using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sharemarketsimulation.DTOs;
using Sharemarketsimulation.Interfaces.Services;

namespace Sharemarketsimulation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User,Broker,Admin")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        [Authorize(Roles = "User")] // Only regular users can place orders
        public async Task<IActionResult> PlaceOrder(CreateOrderDto orderDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized(new { message = "User ID not found in token" });

                var userId = int.Parse(userIdClaim.Value);
                var order = await _orderService.PlaceOrderAsync(userId, orderDto);
                return Ok(order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "User,Admin")] 
        public async Task<IActionResult> GetUserOrders(int userId)
        {
            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }

        [HttpPost("trading-data")]
        public async Task<IActionResult> GetTradingData([FromBody] TradingDataRequest request)
        {
            Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            
            var orders = await _orderService.GetUserOrdersAsync(request.UserId);
            return Ok(new { orders, requestName = request.RequestName });
        }

        public class TradingDataRequest
        {
            public int UserId { get; set; }
            public string RequestName { get; set; } = "TradingData";
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Broker,Admin")] 
        public async Task<IActionResult> GetPendingOrders()
        {
            var orders = await _orderService.GetPendingOrdersAsync();
            return Ok(orders);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var allOrders = await _orderService.GetAllOrdersAsync();
            return Ok(allOrders);
        }

        [HttpPost("execute/{orderId}")]
        [Authorize(Roles = "Broker,Admin")] 
        public async Task<IActionResult> ExecuteOrder(int orderId)
        {
            var success = await _orderService.ExecuteOrderAsync(orderId);
            if (!success)
                return BadRequest(new { message = "Failed to execute order" });

            return Ok(new { message = "Order executed successfully" });
        }

        [HttpDelete("{orderId}")]
        [Authorize(Roles = "User,Admin")] 
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { message = "User ID not found in token" });

            var userId = int.Parse(userIdClaim.Value);
            var success = await _orderService.CancelOrderAsync(orderId, userId);
            if (!success)
                return BadRequest(new { message = "Failed to cancel order" });

            return Ok(new { message = "Order cancelled successfully" });
        }
    }
}