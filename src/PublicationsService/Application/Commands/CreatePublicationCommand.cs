using MediatR;
using PublicationsService.Aplication.Dto;
using SharedKernel.Interface;

namespace PublicationsService.Aplication.Commands
{
    public class CreatePublicationCommand : IRequest<IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        public int IdUser { get; set; }
        public int IdRole { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int Status { get; set; }
        public decimal Salary { get; set; }
        public string Location { get; set; }
        public string Company { get; set; }
        #endregion

        #region Constructor
        public CreatePublicationCommand(PublicationDTO publicationDTO)
        {
            IdUser = publicationDTO.IdUser;
            IdRole = publicationDTO.IdRole;
            Title = publicationDTO.Title;
            Description = publicationDTO.Description;
            ExpirationDate = DateTime.UtcNow.AddDays(28);
            Status = publicationDTO.Status;
            Salary = publicationDTO.Salary;
            Location = publicationDTO.Location;
            Company = publicationDTO.Company;
        }
        #endregion
    }
}
