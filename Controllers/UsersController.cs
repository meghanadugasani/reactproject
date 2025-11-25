using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sharemarketsimulation.DTOs;
using Sharemarketsimulation.Interfaces;
using Sharemarketsimulation.Interfaces.Services;
using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Broker")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize] // Users can view their own profile, Admin can view any
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserProfileAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }

        [HttpPost("user-profile")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile([FromBody] UserProfileRequest request)
        {
            Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            
            var user = await _userService.GetUserProfileAsync(request.UserId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(user);
        }

        public class UserProfileRequest
        {
            public int UserId { get; set; }
            public string RequestName { get; set; } = "UserProfile";
        }

        [HttpPut("{id}")]
        [Authorize] // Users can update their own profile, Admin can update any
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto updateDto)
        {
            try
            {
                var user = await _userService.UpdateProfileAsync(id, updateDto);
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/change-password")]
        [Authorize] // Users can change their own password
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto changePasswordDto)
        {
            var success = await _userService.ChangePasswordAsync(id, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (!success)
                return BadRequest(new { message = "Invalid current password" });

            return Ok(new { message = "Password changed successfully" });
        }

        [HttpGet("role/{role}")]
        [Authorize(Roles = "Admin,Broker")] // Admin and Broker can view users by role
        public async Task<IActionResult> GetUsersByRole(string role)
        {
            if (!Enum.TryParse<UserRole>(role, true, out var userRole))
                return BadRequest(new { message = "Invalid role. Valid values: User, Admin, Broker" });

            var users = await _userService.GetUsersByRoleAsync(userRole);
            return Ok(users);
        }

        [HttpPost("{id}/suspend")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SuspendUser(int id, [FromQuery] string reason)
        {
            var success = await _userService.SuspendUserAsync(id, reason);
            if (!success)
                return NotFound(new { message = "User not found" });

            return Ok(new { message = "User suspended successfully", reason });
        }

        [HttpPost("{id}/reactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReactivateUser(int id)
        {
            var success = await _userService.ReactivateUserAsync(id);
            if (!success)
                return NotFound(new { message = "User not found" });

            return Ok(new { message = "User reactivated successfully" });
        }

        [HttpGet("{id}/balance")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetUserBalance(int id)
        {
            var balance = await _userService.GetUserBalanceAsync(id);
            return Ok(new { userId = id, balance });
        }

        [HttpPost("{id}/update-kyc-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateKYCStatus(int id, [FromQuery] string status)
        {
            if (!Enum.TryParse<KYCStatus>(status, true, out var kycStatus))
                return BadRequest(new { message = "Invalid KYC status. Valid values: Pending, Approved, Rejected, UnderReview" });

            var success = await _userService.UpdateKYCStatusAsync(id, kycStatus);
            if (!success)
                return NotFound(new { message = "User not found" });

            return Ok(new { message = "KYC status updated successfully", status = kycStatus.ToString() });
        }

        [HttpPost("update-kyc-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateKYCStatusPayload([FromBody] UpdateKYCRequest request)
        {
            Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            
            if (!Enum.TryParse<KYCStatus>(request.Status, true, out var kycStatus))
                return BadRequest(new { message = "Invalid KYC status. Valid values: Pending, Approved, Rejected, UnderReview" });

            var success = await _userService.UpdateKYCStatusAsync(request.UserId, kycStatus);
            if (!success)
                return NotFound(new { message = "User not found" });

            return Ok(new { message = "KYC status updated successfully", status = kycStatus.ToString() });
        }

        public class UpdateKYCRequest
        {
            public int UserId { get; set; }
            public string Status { get; set; } = string.Empty;
            public string RequestName { get; set; } = "UpdateKYCStatus";
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            var success = await _userService.DeactivateUserAsync(id);
            if (!success)
                return NotFound(new { message = "User not found" });

            return Ok(new { message = "User deactivated successfully" });
        }
    }

    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}