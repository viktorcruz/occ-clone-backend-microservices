using MediatR;
using PublicationsService.Aplication.DTO;
using SharedKernel.Common.Responses;
using SharedKernel.Interfaces.Response;

namespace PublicationsService.Aplication.Queries
{
    public class GetPublicationByIdQuery : IRequest<IEndpointResponse<RetrieveDatabaseResult<PublicationRetrieveDTO>>>
    {
        #region Properties
        public int IdPublication { get; set; }
        #endregion

        #region Constructor
        public GetPublicationByIdQuery(int publicationId)
        {

            IdPublication = publicationId;

        }
        #endregion
    }
}
