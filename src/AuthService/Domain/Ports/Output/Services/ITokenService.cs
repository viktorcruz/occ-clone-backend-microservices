using AuthService.Application.DTO;
using AuthService.Domain.Entities;
using System.Security.Claims;

namespace AuthService.Domain.Ports.Output.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(LoginUserResponseDTO user);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        string GenerateRefreshToken(UserEntity user);
        ClaimsPrincipal GetPrincipalFromToken(string token);
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string CreateUserAccessToken(UserEntity user);
        bool IsRefreshTokenValid(string refreshToken);
    }
}
