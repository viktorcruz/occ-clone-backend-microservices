using MediatR;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using UsersService.Application.Dto;
using UsersService.Domain.Interface;

namespace UsersService.Application.Queries.Handlers
{
    public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, IEndpointResponse<RetrieveDatabaseResult<List<UserRetrieveDTO>>>>
    {
        #region Properties
        private readonly IUserDomain _userDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<RetrieveDatabaseResult<List<UserRetrieveDTO>>> _endpointResponse;
        #endregion

        #region Constructor
        public SearchUsersQueryHandler(
            IUserDomain userDomain,
            IGlobalExceptionHandler globalExceptionHandler,
            IEndpointResponse<RetrieveDatabaseResult<List<UserRetrieveDTO>>> endpointResponse)
        {
            _userDomain = userDomain;
            _globalExceptionHandler = globalExceptionHandler;
            _endpointResponse = endpointResponse;

        }
        #endregion

        #region Methods
        public async Task<IEndpointResponse<RetrieveDatabaseResult<List<UserRetrieveDTO>>>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {

                var result = await _userDomain.SearchUsersAsync(request.FirstName, request.LastName, request.Email);

                if (result.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "Users retrieved successfully";
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = "No users found";
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "SearchUsersQueryHandler");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = ex.Message;
            }

            return _endpointResponse;
        }
        #endregion
    }
}
