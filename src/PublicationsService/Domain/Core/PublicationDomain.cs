using PublicationsService.Aplication.Commands;
using PublicationsService.Aplication.DTO;
using PublicationsService.Application.DTO;
using PublicationsService.Domain.Interface;
using PublicationsService.Infrastructure.Interface;
using SharedKernel.Common.Responses;

namespace PublicationsService.Domain.Core
{
    public class PublicationDomain : IPublicationDomain
    {
        #region Properties
        public readonly IPublicationRepository _publicationRepository;
        #endregion

        #region Constructor
        public PublicationDomain(IPublicationRepository publicationRepository)
        {
            _publicationRepository = publicationRepository;
        }

        #endregion

        #region Methods
        public async Task<DatabaseResult> CreatePublicationAsync(CreatePublicationCommand command)
        {
            return await _publicationRepository.CreatePublicationAsync(command);
        }

        public async Task<RetrieveDatabaseResult<PublicationRetrieveDTO>> GetPublicationByIdAsync(int publicationId)
        {
            return await _publicationRepository.GetPublicationByIdAsync(publicationId);
        }

        public async Task<RetrieveDatabaseResult<List<PublicationRetrieveDTO>>> GetAllPublicationAsync()
        {
            return await _publicationRepository.GetAllPublicationAsync();
        }

        //public async Task<RetrieveDatabaseResult<List<PublicationRetrieveDTO>>> SearchPublicationAsync(string email)
        //{
        //    return await _publicationRepository.SearchPublicationAsync(email);
        //}

        public async Task<DatabaseResult> UpdatePublicationAsync(PublicationUpdateDTO publicationDTO)
        {
            return await _publicationRepository.UpdatePublicationAsync(publicationDTO);
        }
        public async Task<DatabaseResult> DeletePublicationByIdAsync(int publicationId)
        {
            return await _publicationRepository.DeletePublicationByIdAsync(publicationId);
        }
        #endregion
    }
}
