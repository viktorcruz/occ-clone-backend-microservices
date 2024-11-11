using MediatR;
using SearchJobsService.Application.Dto;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

namespace SearchJobsService.Application.Queries
{
    public class ApplicationQuery : IRequest<IEndpointResponse<RetrieveDatabaseResult<List<UserApplicationsResponseDTO>>>>
    {
        #region Properties
        public int IdUser { get; set; }
        #endregion

        #region Constructor
        public ApplicationQuery(int userId)
        {
            IdUser = userId;
        }
        #endregion
    }
}
