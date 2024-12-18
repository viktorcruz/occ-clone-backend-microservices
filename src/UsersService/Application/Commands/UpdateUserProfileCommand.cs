﻿using MediatR;
using SharedKernel.Interfaces.Response;
using UsersService.Application.DTO;

namespace UsersService.Application.Commands
{
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
