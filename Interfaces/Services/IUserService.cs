using Sharemarketsimulation.DTOs;
using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserProfileDto?> GetUserProfileAsync(int userId);
        Task<UserProfileDto?> AuthenticateAsync(UserLoginDto loginDto);
        Task<UserProfileDto> RegisterAsync(UserRegistrationDto registrationDto);
        Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateUserDto updateDto);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<IEnumerable<UserProfileDto>> GetAllUsersAsync();
        Task<bool> UpdateKYCStatusAsync(int userId, KYCStatus status);
        Task<bool> DeactivateUserAsync(int userId);
        Task<decimal> GetUserBalanceAsync(int userId);
        Task<bool> UpdateBalanceAsync(int userId, decimal amount);
        Task<bool> AddFundsAsync(int userId, decimal amount);
        Task<bool> WithdrawFundsAsync(int userId, decimal amount);
        Task<IEnumerable<UserProfileDto>> GetUsersByRoleAsync(UserRole role);
        Task<IEnumerable<UserProfileDto>> GetPendingKYCUsersAsync();
        Task<bool> ApproveKYCAsync(int userId, int approvedBy);
        Task<bool> RejectKYCAsync(int userId, int rejectedBy, string reason);
        Task<bool> SuspendUserAsync(int userId, string reason);
        Task<bool> ReactivateUserAsync(int userId);
    }
}