﻿using MediatR;
using SharedKernel.Common.Responses;
using SharedKernel.Interfaces.Response;
using UsersService.Application.DTO;

namespace UsersService.Application.Queries
{
    public class SearchUsersQuery : IRequest<IEndpointResponse<RetrieveDatabaseResult<List<SearchUsersDTO>>>>
    {
        #region Properties
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        #endregion

        #region Constructor
        public SearchUsersQuery(string firstName, string lastName, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }
        #endregion
    }
}
