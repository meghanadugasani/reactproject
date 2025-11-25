using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Services
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class NotificationService
    {
        private static readonly List<NotificationDto> _notifications = new();
        private static int _nextId = 1;

        public async Task<List<NotificationDto>> GetUserNotificationsAsync(int userId)
        {
            var userNotifications = _notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
            return await Task.FromResult(userNotifications);
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            var count = _notifications.Count(n => n.UserId == userId && !n.IsRead);
            return await Task.FromResult(count);
        }

        public async Task MarkAsReadAsync(int userId, int notificationId)
        {
            var notification = _notifications.FirstOrDefault(n => n.Id == notificationId && n.UserId == userId);
            if (notification != null)
            {
                notification.IsRead = true;
            }
            await Task.CompletedTask;
        }

        public async Task MarkAllAsReadAsync(int userId)
        {
            var userNotifications = _notifications.Where(n => n.UserId == userId);
            foreach (var notification in userNotifications)
            {
                notification.IsRead = true;
            }
            await Task.CompletedTask;
        }

        public async Task SendPriceAlertAsync(int userId, string stockSymbol, decimal currentPrice, decimal alertPrice, string alertType)
        {
            var title = alertType == "StopLoss" ? "Stop Loss Alert" : "Target Alert";
            var message = alertType == "StopLoss" 
                ? $" {stockSymbol} has dropped to ₹{currentPrice:N2} (Alert level: ₹{alertPrice:N2}). Consider selling to limit losses."
                : $" {stockSymbol} has reached ₹{currentPrice:N2} (Target level: ₹{alertPrice:N2}). Consider selling to book profits.";

            _notifications.Add(new NotificationDto
            {
                Id = _nextId++,
                UserId = userId,
                Title = title,
                Message = message,
                Type = alertType,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });

            await Task.CompletedTask;
        }

        public async Task SendOrderExecutedNotificationAsync(int userId, string stockSymbol, string orderType, int quantity, decimal price)
        {
            var message = $" {orderType} order executed: {quantity} shares of {stockSymbol} at ₹{price:N2}";
            
            _notifications.Add(new NotificationDto
            {
                Id = _nextId++,
                UserId = userId,
                Title = "Order Executed",
                Message = message,
                Type = "OrderExecuted",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });

            await Task.CompletedTask;
        }
    }
}