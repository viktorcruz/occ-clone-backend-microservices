using MediatR;
using SharedKernel.Interface;
using UsersService.Application.Dto;

namespace UsersService.Application.Commands
{
    public class ChangeUserPasswordCommand : IRequest<IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        public int IdUser { get; set; }
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        #endregion

        #region Constructor
        public ChangeUserPasswordCommand(UserPasswordDTO userPasswordDTO)
        {
            IdUser = userPasswordDTO.IdUser;
            Email = userPasswordDTO.Email;
            CurrentPassword = userPasswordDTO.CurrentPassword;
            NewPassword = userPasswordDTO.NewPassword;
        }
        #endregion
    }
}
