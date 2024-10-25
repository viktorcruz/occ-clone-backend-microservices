using PublicationsService.Aplication.Dto;
using SharedKernel.Common.Responses;

namespace PublicationsService.Infrastructure.Interface
{
    public interface IPublicationRepository
    {
        Task<DatabaseResult> CreatePublishAsync(PublicationDTO publicationDTO);
        Task<RetrieveDatabaseResult<PublicationRetrieveDTO>> GetPublicationByIdAsync(int publicationId);
        Task<RetrieveDatabaseResult<List<PublicationRetrieveDTO>>> GetAllPublicationAsync();
        //Task<RetrieveDatabaseResult<List<PublicationRetrieveDTO>>> SearchPublicationAsync();
        Task<DatabaseResult> UpdatePublicationAsync(PublicationDTO publicationDTO);
        Task<DatabaseResult> DeletePublicationByIdAsync(int publicationId);
    }
}
