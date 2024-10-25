using PublicationsService.Aplication.Dto;
using PublicationsService.Domain.Interface;
using SharedKernel.Common.Responses;

namespace PublicationsService.Domain.Core
{
    public class PublicationDomain : IPublicationDomain
    {
        #region Properties

        #endregion

        #region Constructor
        public PublicationDomain()
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
