using MediatR;
using SharedKernel.Interface;
using UsersService.Application.Dto;

namespace UsersService.Application.Commands
{
    /// <summary>
    /// TODO: Actualiza el perfil de un usuario, lo que es mas claro que un generico update
    /// </summary>
    public class UpdateUserProfileCommand : IRequest<IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        public int IdUser { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        #endregion

        #region Constructor
        public UpdateUserProfileCommand(UserProfileDTO userProfileDTO)
        {
            IdUser = userProfileDTO.IdUser;
            FirstName = userProfileDTO.FirstName;
            LastName = userProfileDTO.LastName;
            Email = userProfileDTO.Email;
        }
        #endregion
    }
}
