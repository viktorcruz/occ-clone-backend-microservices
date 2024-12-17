using PublicationsService.Aplication.Commands;
using PublicationsService.Aplication.DTO;
using PublicationsService.Application.DTO;
using SharedKernel.Common.Responses;

namespace PublicationsService.Domain.Interface
{
    public interface IPublicationDomain
    {
        Task<DatabaseResult> CreatePublicationAsync(CreatePublicationCommand command);
        Task<RetrieveDatabaseResult<PublicationRetrieveDTO>> GetPublicationByIdAsync(int publicationId);
        Task<RetrieveDatabaseResult<List<PublicationRetrieveDTO>>> GetAllPublicationAsync();
        Task<DatabaseResult> UpdatePublicationAsync(PublicationUpdateDTO publicationDTO);
        Task<DatabaseResult> DeletePublicationByIdAsync(int publicationId);
    }
}
