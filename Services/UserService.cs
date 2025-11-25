using Sharemarketsimulation.DTOs;
using Sharemarketsimulation.Interfaces;
using Sharemarketsimulation.Interfaces.Repository;
using Sharemarketsimulation.Interfaces.Services;
using Sharemarketsimulation.Models;


namespace Sharemarketsimulation.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            return new UserProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Password = user.Password,
                Role = user.Role,
                KYCStatus = user.KYCStatus,
                Balance = user.Balance,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive,
                AccountMode = DetermineAccountMode(user)
            };
        }

        public async Task<UserProfileDto?> AuthenticateAsync(UserLoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null || user.Password != loginDto.Password || !user.IsActive)
                return null;

            return await GetUserProfileAsync(user.Id);
        }

        public async Task<UserProfileDto> RegisterAsync(UserRegistrationDto registrationDto)
        {
            if (await _userRepository.EmailExistsAsync(registrationDto.Email))
                throw new InvalidOperationException("Email already exists");

            var user = new User
            {
                FirstName = registrationDto.FirstName,
                LastName = registrationDto.LastName,
                Email = registrationDto.Email,
                Password = registrationDto.Password,
                PhoneNumber = registrationDto.PhoneNumber,
                Role = UserRole.User,
                KYCStatus = KYCStatus.Pending,
                Balance = 100000,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var createdUser = await _userRepository.CreateAsync(user);
            return await GetUserProfileAsync(createdUser.Id) ?? throw new InvalidOperationException("Failed to create user");
        }

        public async Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateUserDto updateDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new ArgumentException("User not found");

            if (!string.IsNullOrEmpty(updateDto.FirstName))
                user.FirstName = updateDto.FirstName;
            if (!string.IsNullOrEmpty(updateDto.LastName))
                user.LastName = updateDto.LastName;
            if (!string.IsNullOrEmpty(updateDto.PhoneNumber))
                user.PhoneNumber = updateDto.PhoneNumber;
            if (!string.IsNullOrEmpty(updateDto.AccountMode))
            {
                // Handle account mode change by adjusting balance and KYC
                if (updateDto.AccountMode == "Virtual" && DetermineAccountMode(user) == "Real")
                {
                    user.Balance = 10000; 
                    user.KYCStatus = KYCStatus.Pending; 
                }
                else if (updateDto.AccountMode == "Real" && DetermineAccountMode(user) == "Virtual")
                {
                    user.Balance = 0;
                }
            }

            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return await GetUserProfileAsync(userId) ?? throw new InvalidOperationException("Failed to update user");
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.Password != currentPassword)
                return false;

            user.Password = newPassword;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<IEnumerable<UserProfileDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new UserProfileDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Password = u.Password,
                Role = u.Role,
                KYCStatus = u.KYCStatus,
                Balance = u.Balance,
                CreatedAt = u.CreatedAt,
                IsActive = u.IsActive,
                AccountMode = DetermineAccountMode(u)
            });
        }

        public async Task<bool> UpdateKYCStatusAsync(int userId, KYCStatus status)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            user.KYCStatus = status;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> DeactivateUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<decimal> GetUserBalanceAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user?.Balance ?? 0;
        }

        public async Task<bool> UpdateBalanceAsync(int userId, decimal amount)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            user.Balance += amount;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> AddFundsAsync(int userId, decimal amount)
        {
            return await UpdateBalanceAsync(userId, amount);
        }

        public async Task<bool> WithdrawFundsAsync(int userId, decimal amount)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.Balance < amount) return false;
            return await UpdateBalanceAsync(userId, -amount);
        }

        public async Task<IEnumerable<UserProfileDto>> GetUsersByRoleAsync(UserRole role)
        {
            var users = await _userRepository.GetByRoleAsync(role);
            return users.Select(u => new UserProfileDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Role,
                KYCStatus = u.KYCStatus,
                Balance = u.Balance,
                CreatedAt = u.CreatedAt,
                IsActive = u.IsActive
            });
        }

        public async Task<IEnumerable<UserProfileDto>> GetPendingKYCUsersAsync()
        {
            var users = await _userRepository.GetByKYCStatusAsync(KYCStatus.Pending);
            return users.Select(u => new UserProfileDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Role,
                KYCStatus = u.KYCStatus,
                CreatedAt = u.CreatedAt
            });
        }

        public async Task<bool> ApproveKYCAsync(int userId, int approvedBy)
        {
            return await UpdateKYCStatusAsync(userId, KYCStatus.Approved);
        }

        public async Task<bool> RejectKYCAsync(int userId, int rejectedBy, string reason)
        {
            return await UpdateKYCStatusAsync(userId, KYCStatus.Rejected);
        }

        public async Task<bool> SuspendUserAsync(int userId, string reason)
        {
            return await DeactivateUserAsync(userId);
        }

        public async Task<bool> ReactivateUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            return true;
        }
        
        private static string DetermineAccountMode(User user)
        {
            // Same logic as AuthService
            if (user.KYCStatus == KYCStatus.Approved)
                return "Real";
                
            if (user.Balance != 100000 && user.Balance != 10000)
                return "Real";
                
            if (user.Balance == 10000)
                return "Virtual";
                
            return "Virtual";
        }
    }
}