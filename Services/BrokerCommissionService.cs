using Sharemarketsimulation.Interfaces.Repository;
using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Services
{
    public class BrokerCommissionService
    {
        private readonly IUserRepository _userRepository;
        
        public BrokerCommissionService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        
        
        public async Task<bool> PayCommissionToBrokerAsync(int brokerId, decimal commission, string accountMode)
        {
            if (accountMode == "Virtual")
            {
                
                return false;
            }
            
          
            var broker = await _userRepository.GetByIdAsync(brokerId);
            if (broker == null || broker.Role != UserRole.Broker)
            {
                return false;
            }
            
            
            broker.Balance += commission;
            broker.UpdatedAt = DateTime.UtcNow;
            
            await _userRepository.UpdateAsync(broker);
            
            
            await LogCommissionEarningAsync(brokerId, commission);
            
            return true;
        }
        
       
        public async Task<decimal> GetBrokerCommissionEarningsAsync(int brokerId)
        {
           
            var broker = await _userRepository.GetByIdAsync(brokerId);
            return broker?.Balance ?? 0;
        }
        
     
        public async Task<IEnumerable<object>> GetAllBrokerEarningsAsync()
        {
            var brokers = await _userRepository.GetByRoleAsync(UserRole.Broker);
            
            return brokers.Select(b => new
            {
                BrokerId = b.Id,
                Name = $"{b.FirstName} {b.LastName}",
                Email = b.Email,
                TotalEarnings = b.Balance,
                IsActive = b.IsActive
            });
        }
        
        private async Task LogCommissionEarningAsync(int brokerId, decimal commission)
        {
            
            Console.WriteLine($" REAL COMMISSION PAID: Broker {brokerId} received ₹{commission} at {DateTime.UtcNow}");
        }
    }
}