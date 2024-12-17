using AuthService.Application.Commands;
using AuthService.Application.DTO;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Ports.Output;
using AuthService.Domain.Ports.Output.Services;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Interfaces.Exceptions;
using System.Security.Claims;

namespace AuthService.Application.UsesCases
{
    public class RenewTokenUseCase : IRenewTokenUseCase
    {
        private readonly ITokenService _tokenService;
        private readonly IRenewTokenPort _renewTokenPort;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;

        public RenewTokenUseCase(ITokenService tokenService, IRenewTokenPort renewTokenPort, IApplicationExceptionHandler applicationExceptionHandler)
        {
            _tokenService = tokenService;
            _renewTokenPort = renewTokenPort;
            _applicationExceptionHandler = applicationExceptionHandler;
        }

        public async Task<RenewTokenDTO> Execute(RenewCommand command)
        {
            try
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
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Application, ActionType.Renew);
                throw;
            }
        }
    }
}
