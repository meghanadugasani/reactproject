using Sharemarketsimulation.Models;

namespace Sharemarketsimulation.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user, string accountMode = "Virtual");
        bool ValidateToken(string token);
        int? GetUserIdFromToken(string token);
    }
}