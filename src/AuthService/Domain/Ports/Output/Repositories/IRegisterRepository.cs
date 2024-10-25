using AuthService.Domain.Entities;
using SharedKernel.Common.Responses;

namespace AuthService.Domain.Ports.Output.Repositories
{
    public interface IRegisterRepository
    {
        Task<DatabaseResult> AddAsync(RegisterEntity registerEntity);
    }
}
