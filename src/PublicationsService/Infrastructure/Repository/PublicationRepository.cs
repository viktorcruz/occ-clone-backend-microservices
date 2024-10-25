using PublicationsService.Aplication.Dto;
using PublicationsService.Infrastructure.Interface;
using SharedKernel.Common.Responses;

namespace PublicationsService.Infrastructure.Repository
{
    public class PublicationRepository : IPublicationRepository
    {
        #region Properties

        #endregion

        #region Constructor
        public PublicationRepository()
        {

        }
        #endregion

        #region Methods
        public Task<DatabaseResult> CreatePublishAsync(PublicationDTO publicationDTO)
        {
            throw new NotImplementedException();
        }

        public Task<RetrieveDatabaseResult<PublicationRetrieveDTO>> GetPublicationByIdAsync(int publicationId)
        {
            throw new NotImplementedException();
        }

        public Task<RetrieveDatabaseResult<List<PublicationRetrieveDTO>>> GetAllPublicationAsync()
        {
            throw new NotImplementedException();
        }

        public Task<DatabaseResult> UpdatePublicationAsync(PublicationDTO publicationDTO)
        {
            throw new NotImplementedException();
        }

        public Task<DatabaseResult> DeletePublicationByIdAsync(int publicationId)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
