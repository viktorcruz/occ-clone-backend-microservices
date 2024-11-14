using AuthService.Domain.Entities;
using SharedKernel.Common.Events;
using SharedKernel.Common.Responses;

namespace AuthService.Domain.Ports.Output
{
    public interface IRegisterUserPort
    {
        Task<DatabaseResult> AddAsync(RegisterEntity registerEntity);
    }
}
