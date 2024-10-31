using MediatR;
using PublicationsService.Aplication.Dto;
using PublicationsService.Application.Dto;
using SharedKernel.Interface;

namespace PublicationsService.Aplication.Commands
{
    public class UpdatePublicationCommand : IRequest<IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        public int IdPubliaction { get; set; }
        public int IdUser { get; set; }
        public int IdRole { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public decimal Salary { get; set; }
        public string Location { get; set; }
        public string Company { get; set; }
        #endregion

        #region Constructor
        public UpdatePublicationCommand(PublicationUpdateDTO retrieveDTO)
        {
            IdPubliaction = retrieveDTO.IdPublication;
            IdUser = retrieveDTO.IdUser;
            IdRole = retrieveDTO.IdRole;
            Title = retrieveDTO.Title;
            Description = retrieveDTO.Description;
            Status = retrieveDTO.Status;
            Salary = retrieveDTO.Salary;
            Location = retrieveDTO.Location;
            Company = retrieveDTO.Company;
        }
        #endregion
    }
}
