using MediatR;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using System.CodeDom;
using UsersService.Application.Dto;

namespace UsersService.Application.Queries
{
    /// <summary>
    /// Permite buscar usuarios basados en ciertos ciriterios, lo que es mas flexible que un simple getAll
    /// </summary>
    public class SearchUsersQuery : IRequest<IEndpointResponse<RetrieveDatabaseResult<List<UserRetrieveDTO>>>>
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
