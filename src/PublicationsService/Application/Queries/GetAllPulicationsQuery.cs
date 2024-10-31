using MediatR;
using PublicationsService.Aplication.Dto;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

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
