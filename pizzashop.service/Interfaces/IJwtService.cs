using System.Security.Claims;

namespace pizzashop.service
{
    public interface IJwtService
    {
        string GenerateJwtToken(string email, string role);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
