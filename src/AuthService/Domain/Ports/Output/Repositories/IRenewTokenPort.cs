using AuthService.Application.Commands;
using AuthService.Application.DTO;

namespace AuthService.Domain.Ports.Output.Repositories
{
    public interface IRenewTokenPort
    {
        Task<RenewTokenResponseDTO> RenewAsync(string emailClaim);
    }
}
