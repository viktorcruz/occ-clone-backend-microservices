using AuthService.Application.Commands;
using AuthService.Application.DTO;

namespace AuthService.Domain.Ports.Output
{
    public interface IRenewTokenPort
    {
        Task<RenewTokenResponseDTO> RenewAsync(string emailClaim);
    }
}
