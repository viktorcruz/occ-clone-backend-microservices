using MediatR;
using SharedKernel.Interface;
using UsersService.Application.Dto;

namespace UsersService.Application.Commands
{
    public class UpdateUserCommand : IRequest<IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        public int IdUser { get; set; }
        public int IdRole { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime RegistratioNConfirmedAt { get; set; }
        public int IsRegistrationConfirmed { get; set; }
        public int IsActive { get; set; }
        #endregion

        #region Constructor
        public UpdateUserCommand(UserRetrieveDTO userDTO)
        {
            IdUser = userDTO.IdUser;
            IdRole = userDTO.IdRole;
            FirstName = userDTO.FirstName;
            LastName = userDTO.LastName;
            Email = userDTO.Email;
        }
        #endregion
    }
}
