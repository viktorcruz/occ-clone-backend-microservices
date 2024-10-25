using AuthService.Application.Commands;
using AuthService.Application.DTO;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Ports.Output.Repositories;
using AuthService.Domain.Ports.Output.Services;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace AuthService.Application.UsesCases
{
    public class RenewTokenUseCase : IRenewTokenUseCase
    {
        private readonly ITokenService _tokenService;
        private readonly IRenewTokenPort _renewTokenPort;

        public RenewTokenUseCase(ITokenService tokenService, IRenewTokenPort renewTokenPort)
        {
            _tokenService = tokenService;
            _renewTokenPort = renewTokenPort;
        }

        public async Task<RenewTokenDTO> Execute(RenewCommand command)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(command.RefreshToken);
            var userEmailClaim = principal?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (userEmailClaim == null)
            {
                throw new SecurityTokenException("Invalid refresh token");
            }
            var user = await _renewTokenPort.RenewAsync(userEmailClaim);

            if (user.Email == null)
            {
                throw new SecurityTokenException("Invalid refresh token");
            }

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);

            var newRefreshToken = _tokenService.GenerateRefreshToken(new UserEntity
            {
                IdUser = user.IdUser,
                Email = userEmailClaim
            });

            return new RenewTokenDTO
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            };
        }
    }
}
