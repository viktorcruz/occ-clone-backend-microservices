using AuthService.Application.Commands;
using AuthService.Application.DTO;

namespace AuthService.Application.Interfaces
{
    public interface  IRenewTokenUseCase
    {
        Task<RenewTokenDTO> Execute(RenewCommand command);
    }
}
