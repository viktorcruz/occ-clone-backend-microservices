using MediatR;
using PublicationsService.Aplication.Dto;
using SharedKernel.Interface;

namespace PublicationsService.Aplication.Commands
{
    public class CreatePublicationCommand : IRequest<IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        public string Title { get; set; }
        public string Description { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public DateTime PostedDate { get; set; }
        public int IdUser { get; set; }
        #endregion

        #region Constructor
        public CreatePublicationCommand(PublicationDTO publicationDTO)
        {
            Title = publicationDTO.Title;
            Description = publicationDTO.Description;
            Company = publicationDTO.Company;
            Location = publicationDTO.Location;
            PostedDate = publicationDTO.PostedDate;
            IdUser = publicationDTO.IdUser;
        }
        #endregion

        #region Methods

        #endregion
    }
}
