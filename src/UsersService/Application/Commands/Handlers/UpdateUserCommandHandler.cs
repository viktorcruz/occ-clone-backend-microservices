using MediatR;
using SharedKernel.Interface;
using UsersService.Application.Dto;
using UsersService.Domain.Interface;

namespace UsersService.Application.Commands.Handlers
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly IUserDomain _usersDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        #endregion

        #region Constructor
        public UpdateUserCommandHandler(IUserDomain usersDomain, IGlobalExceptionHandler globalExceptionHandler, IEndpointResponse<IDatabaseResult> endpointResponse)
        {
            _usersDomain = usersDomain;
            _globalExceptionHandler = globalExceptionHandler;
            _endpointResponse = endpointResponse;

        }
        #endregion

        public async Task<IEndpointResponse<IDatabaseResult>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = new UserRetrieveDTO
                {
                    IdUser = request.IdUser,
                    IdRole = request.IdRole,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    IsActive = request.IsActive,
                };

                var response = await _usersDomain.UpdateUserAsync(user);
                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "User updated sucessful";
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = response?.ResultMessage ?? "User not found";
                }
                return _endpointResponse;
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "UpdateUserCommandHandler");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = $"Error updating user: {ex.Message}";
                return _endpointResponse;
            }
        }
    }
}
