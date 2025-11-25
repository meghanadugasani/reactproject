using Sharemarketsimulation.DTOs;
using Sharemarketsimulation.Interfaces.Repository;
using Sharemarketsimulation.Interfaces.Services;
using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AuthService(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null || user.Password != loginDto.Password)
                throw new UnauthorizedAccessException("Invalid credentials");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("Account is deactivated");

            // Determine account mode based on existing user data
            var accountMode = DetermineAccountMode(user);

            var token = _tokenService.GenerateToken(user, accountMode);
            var availableBalance = accountMode == "Virtual" ? user.Balance : 0;

            var userDto = MapToUserDto(user);
            userDto.AccountMode = accountMode; // Ensure AccountMode is set correctly
            
            return new AuthResponseDto
            {
                Token = token,
                User = userDto,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                AccountMode = accountMode,
                AvailableBalance = availableBalance,
                BalanceType = accountMode == "Virtual" ? "Virtual Money (Practice)" : "Real Money"
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
            if (existingUser != null)
                throw new InvalidOperationException("Email already registered");

            if (registerDto.AccountMode != "Virtual" && registerDto.AccountMode != "Real")
                throw new ArgumentException("Invalid account mode. Choose 'Virtual' or 'Real'");

            var startingBalance = registerDto.AccountMode == "Virtual" ? 10000 : 0;

            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                Password = registerDto.Password,
                Role = UserRole.User,
                IsActive = true,
                Balance = startingBalance,
                KYCStatus = registerDto.AccountMode == "Virtual" ? KYCStatus.Approved : KYCStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.CreateAsync(user);
            var token = _tokenService.GenerateToken(createdUser, registerDto.AccountMode);

            var userDto = MapToUserDto(createdUser);
            userDto.AccountMode = registerDto.AccountMode; // Ensure AccountMode is set correctly
            
            return new AuthResponseDto
            {
                Token = token,
                User = userDto,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                AccountMode = registerDto.AccountMode,
                AvailableBalance = startingBalance,
                BalanceType = registerDto.AccountMode == "Virtual" ? "Virtual Money (Practice)" : "Real Money"
            };
        }

        public async Task<AuthResponseDto> RegisterStaffAsync(RegisterStaffDto registerDto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
            if (existingUser != null)
                throw new InvalidOperationException("Email already registered");

            var userRole = registerDto.Role.Equals("Broker", StringComparison.OrdinalIgnoreCase) 
                ? UserRole.Broker 
                : UserRole.Admin;

            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                Password = registerDto.Password,
                Role = userRole,
                IsActive = true,
                Balance = 0,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.CreateAsync(user);
            var staffAccountMode = userRole == UserRole.Admin ? "Admin" : "Broker";
            var token = _tokenService.GenerateToken(createdUser, staffAccountMode);

            return new AuthResponseDto
            {
                Token = token,
                User = MapToUserDto(createdUser),
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                AccountMode = staffAccountMode,
                AvailableBalance = 0,
                BalanceType = userRole == UserRole.Admin ? "Admin Account" : "Broker Account"
            };
        }




        private static string DetermineAccountMode(User user)
        {
            // If user has KYC approved, they are Real Money user
            if (user.KYCStatus == KYCStatus.Approved)
                return "Real";
                
            // If user has balance of 0, they are Real Money user (registered as Real)
            if (user.Balance == 0)
                return "Real";
                
            // If user has balance of 10000, they are Virtual Money user (registered as Virtual)
            if (user.Balance == 10000)
                return "Virtual";
                
            // If user has any other balance, they are Real Money user
            return "Real";
        }

        private static UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName ?? "",
                LastName = user.LastName ?? "",
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber ?? "",
                Role = user.Role,
                Balance = user.Balance,
                IsActive = user.IsActive,
                KYCStatus = user.KYCStatus,
                CreatedAt = user.CreatedAt
            };
        }
    }
}