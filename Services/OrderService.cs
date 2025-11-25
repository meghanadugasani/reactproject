using Sharemarketsimulation.DTOs;
using Sharemarketsimulation.Interfaces;
using Sharemarketsimulation.Interfaces.Repository;
using Sharemarketsimulation.Interfaces.Services;
using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IStockRepository _stockRepository;
        private readonly IPortfolioService _portfolioService;
        private readonly MarketTimingService _marketTimingService;
        private readonly ITransactionService _transactionService;

        public OrderService(IOrderRepository orderRepository, IUserRepository userRepository, 
            IStockRepository stockRepository, IPortfolioService portfolioService, MarketTimingService marketTimingService, ITransactionService transactionService)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _stockRepository = stockRepository;
            _portfolioService = portfolioService;
            _marketTimingService = marketTimingService;
            _transactionService = transactionService;
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            return order == null ? null : MapToDto(order);
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync(string accountMode = "Virtual")
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Select(order => MapToDto(order, accountMode));
        }

        public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId, string accountMode = "Virtual")
        {
            var orders = await _orderRepository.GetByUserIdAsync(userId);
            return orders.Select(order => MapToDto(order, accountMode));
        }

        public async Task<OrderDto> PlaceOrderAsync(int userId, CreateOrderDto orderDto, string accountMode = "Virtual")
        {
           
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");
           
            var userAccountMode = DetermineAccountMode(user);
    
            var (tradingAllowed, message) = _marketTimingService.ValidateTrading(userAccountMode);
            if (!tradingAllowed)
                throw new InvalidOperationException(message);
       
            var stock = await _stockRepository.GetByIdAsync(orderDto.StockId);
            if (stock == null || !stock.IsActive)
                throw new InvalidOperationException("Stock not found or inactive");
            
            var currentPrice = stock.CurrentPrice;
            var totalCost = orderDto.Quantity * currentPrice;
                
            if (orderDto.OrderType == OrderType.Buy)
            {
                // Skip KYC checks for Virtual users
                if (userAccountMode == "Real" && user.KYCStatus != KYCStatus.Approved)
                {
                    throw new InvalidOperationException(
                        "KYC verification required. Please upload your documents (Aadhaar, PAN, Bank Statement) " +
                        "and complete KYC verification before buying stocks. Visit Profile > Documents to upload.");
                }
                
               
                if (user.Balance < totalCost)
                {
                    if (userAccountMode == "Virtual")
                    {
                        throw new InvalidOperationException(
                            $"Insufficient virtual balance. Required: ₹{totalCost:N2}, Available: ₹{user.Balance:N2}. " +
                            $"Virtual users start with ₹10,000 practice money.");
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"Insufficient real money balance. Required: ₹{totalCost:N2}, Available: ₹{user.Balance:N2}. " +
                            $"Please deposit real money to your account first using UPI/Net Banking/Cards.");
                    }
                }
            }

            var order = new Order
            {
                UserId = userId,
                StockId = orderDto.StockId,
                OrderType = orderDto.OrderType,
                Quantity = orderDto.Quantity,
                Price = currentPrice,
                StopLoss = orderDto.StopLoss,
                Target = orderDto.Target,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = orderDto.ExpiresAt,
                Notes = orderDto.Notes
            };

            var createdOrder = await _orderRepository.CreateAsync(order);
            
          
            if (orderDto.OrderType == OrderType.Buy || orderDto.OrderType == OrderType.Sell)
            {
                await ExecuteOrderAsync(createdOrder.Id);
                
                createdOrder = await _orderRepository.GetByIdAsync(createdOrder.Id) ?? createdOrder;
            }
            
            return MapToDto(createdOrder, userAccountMode);
        }

        public async Task<OrderDto> UpdateOrderAsync(int orderId, UpdateOrderDto updateDto)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) throw new ArgumentException("Order not found");

            if (order.Status != OrderStatus.Pending)
                throw new InvalidOperationException("Can only update pending orders");

            if (updateDto.Quantity.HasValue) order.Quantity = updateDto.Quantity.Value;
            if (updateDto.Price.HasValue) order.Price = updateDto.Price.Value;
            if (updateDto.StopLoss.HasValue) order.StopLoss = updateDto.StopLoss.Value;
            if (updateDto.Target.HasValue) order.Target = updateDto.Target.Value;
            if (updateDto.ExpiresAt.HasValue) order.ExpiresAt = updateDto.ExpiresAt.Value;
            if (!string.IsNullOrEmpty(updateDto.Notes)) order.Notes = updateDto.Notes;

            var updatedOrder = await _orderRepository.UpdateAsync(order);
            return MapToDto(updatedOrder);
        }

        public async Task<bool> CancelOrderAsync(int orderId, int userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null || order.UserId != userId) return false;

            if (order.Status != OrderStatus.Pending)
                return false;

            return await _orderRepository.UpdateStatusAsync(orderId, OrderStatus.Cancelled);
        }

        public async Task<IEnumerable<OrderDto>> GetPendingOrdersAsync()
        {
            var orders = await _orderRepository.GetPendingOrdersAsync();
            return orders.Select(order => MapToDto(order));
        }

        public async Task<bool> ExecuteOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null || order.Status != OrderStatus.Pending) return false;

        
            var totalCost = order.Quantity * order.Price;
            
            if (order.OrderType == OrderType.Buy)
            {
               
                var user = await _userRepository.GetByIdAsync(order.UserId);
                if (user != null && user.Balance >= totalCost)
                {
                    user.Balance -= totalCost;
                    user.UpdatedAt = DateTime.UtcNow;
                    await _userRepository.UpdateAsync(user);
                    
                    await _portfolioService.UpdatePortfolioAsync(order.UserId, order.StockId, order.Quantity, order.Price, true);
                    
                    // Create transaction record
                    await _transactionService.CreateTransactionAsync(order.UserId, order.StockId, order.Id, TransactionType.Buy, order.Quantity, order.Price);
                }
                else
                {
                    // Insufficient balance 
                    return false;
                }
            }
            else 
            {
                //   sell order Add money to user account
                var user = await _userRepository.GetByIdAsync(order.UserId);
                if (user != null)
                {
                    user.Balance += totalCost;
                    user.UpdatedAt = DateTime.UtcNow;
                    await _userRepository.UpdateAsync(user);
                    
                    await _portfolioService.UpdatePortfolioAsync(order.UserId, order.StockId, order.Quantity, order.Price, false);
                    
                    // Create transaction record
                    await _transactionService.CreateTransactionAsync(order.UserId, order.StockId, order.Id, TransactionType.Sell, order.Quantity, order.Price);
                }
            }

            
            await _orderRepository.UpdateStatusAsync(orderId, OrderStatus.Executed);
            return true;
        }

        public async Task ProcessExpiredOrdersAsync()
        {
            var expiredOrders = await _orderRepository.GetExpiredOrdersAsync();
            foreach (var order in expiredOrders)
            {
                await _orderRepository.UpdateStatusAsync(order.Id, OrderStatus.Expired);
            }
        }

        public async Task<bool> ValidateOrderAsync(int userId, CreateOrderDto orderDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !user.IsActive) return false;

            var stock = await _stockRepository.GetByIdAsync(orderDto.StockId);
            if (stock == null || !stock.IsActive) return false;

            if (orderDto.OrderType == OrderType.Buy)
            {
                
                return true;
            }
            else
            {
                return await _portfolioService.HasSufficientStocksAsync(userId, orderDto.StockId, orderDto.Quantity);
            }
        }
        
        private static string DetermineAccountMode(User user)
        {
            if (user.KYCStatus == KYCStatus.Approved)
                return "Real";
                
            if (user.Balance != 100000 && user.Balance != 10000)
                return "Real";
                
            if (user.Balance == 10000)
                return "Virtual";
                
            return "Virtual";
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(OrderStatus status)
        {
            var orders = await _orderRepository.GetByStatusAsync(status);
            return orders.Select(order => MapToDto(order));
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByStockAsync(int stockId)
        {
            var orders = await _orderRepository.GetByStockIdAsync(stockId);
            return orders.Select(order => MapToDto(order));
        }

        public async Task<bool> ExecuteMarketOrdersAsync()
        {
            var pendingOrders = await _orderRepository.GetPendingOrdersAsync();
            foreach (var order in pendingOrders)
            {
                await ExecuteOrderAsync(order.Id);
            }
            return true;
        }

        public async Task<decimal> GetOrderValueAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            return order?.Quantity * order.Price ?? 0;
        }

        public async Task<IEnumerable<OrderDto>> GetOrderHistoryAsync(int userId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var orders = await _orderRepository.GetByUserIdAsync(userId);
            if (fromDate.HasValue)
                orders = orders.Where(o => o.CreatedAt >= fromDate.Value);
            if (toDate.HasValue)
                orders = orders.Where(o => o.CreatedAt <= toDate.Value);
            return orders.Select(order => MapToDto(order));
        }

        public async Task<bool> ModifyOrderPriceAsync(int orderId, decimal newPrice)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null || order.Status != OrderStatus.Pending) return false;
            order.Price = newPrice;
            await _orderRepository.UpdateAsync(order);
            return true;
        }

        public async Task<bool> ModifyOrderQuantityAsync(int orderId, int newQuantity)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null || order.Status != OrderStatus.Pending) return false;
            order.Quantity = newQuantity;
            await _orderRepository.UpdateAsync(order);
            return true;
        }

        private static OrderDto MapToDto(Order order, string accountMode = "Virtual")
        {
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                StockId = order.StockId,
                StockSymbol = order.Stock?.Symbol ?? "",
                CompanyName = order.Stock?.CompanyName ?? "",
                OrderType = order.OrderType,
                Quantity = order.Quantity,
                Price = order.Price,
                StopLoss = order.StopLoss,
                Target = order.Target,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                ExecutedAt = order.ExecutedAt,
                ExpiresAt = order.ExpiresAt,
                Notes = order.Notes,
                AccountMode = accountMode
            };
        }
    }
}