using MediatR;
using PublicationsService.Aplication.DTO;
using SharedKernel.Common.Responses;
using SharedKernel.Interfaces.Response;

namespace PublicationsService.Aplication.Queries
{
    public class GetAllPulicationsQuery : IRequest<IEndpointResponse<RetrieveDatabaseResult<List<PublicationRetrieveDTO>>>>
    {
        #region Constructor
        public GetAllPulicationsQuery()
        {
            
        }
        #endregion
    }
}
