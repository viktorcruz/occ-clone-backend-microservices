using PublicationsService.Aplication.Commands;
using PublicationsService.Aplication.Dto;
using PublicationsService.Application.Dto;
using SharedKernel.Common.Responses;

namespace PublicationsService.Infrastructure.Interface
{
    public interface IPublicationRepository
    {
        Task<DatabaseResult> CreatePublicationAsync(CreatePublicationCommand command);
        Task<RetrieveDatabaseResult<PublicationRetrieveDTO>> GetPublicationByIdAsync(int publicationId);
        Task<RetrieveDatabaseResult<List<PublicationRetrieveDTO>>> GetAllPublicationAsync();
        //Task<RetrieveDatabaseResult<List<PublicationRetrieveDTO>>> SearchPublicationAsync(string email);
        Task<DatabaseResult> UpdatePublicationAsync(PublicationUpdateDTO publicationDTO);
        Task<DatabaseResult> DeletePublicationByIdAsync(int publicationId);
    }
}
