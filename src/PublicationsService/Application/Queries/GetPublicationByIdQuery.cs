using MediatR;
using PublicationsService.Aplication.Dto;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

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
