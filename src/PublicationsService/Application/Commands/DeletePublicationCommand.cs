using MediatR;
using SharedKernel.Interface;

namespace PublicationsService.Aplication.Commands
{
    public class DeletePublicationCommand : IRequest<IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        public int IdPublication { get; set; }
        #endregion

        #region Constructor
        public DeletePublicationCommand(int publicationId)
        {
            IdPublication = publicationId;  
        }
        #endregion
    }
}
