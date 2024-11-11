using MediatR;
using SearchJobsService.Application.Dto;
using SharedKernel.Interface;

namespace SearchJobsService.Application.Commands
{
    public class WithdrawCommand : IRequest<IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        public int IdUser { get; set; }
        public int IdPublication { get; set; }
        #endregion

        #region Constructor
        public WithdrawCommand(WithdrawApplicationRequestDTO withdrawDto)
        {
            IdUser = withdrawDto.IdUser;
            IdPublication = withdrawDto.IdPublication;
        }
        #endregion
    }
}
