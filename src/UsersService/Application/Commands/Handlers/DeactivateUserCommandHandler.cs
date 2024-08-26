using MediatR;
using SharedKernel.Interface;
using UsersService.Application.Dto;
using UsersService.Domain.Interface;

namespace UsersService.Application.Commands.Handlers
{
    public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly IUserDomain _userDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        #endregion

        #region Constructor
        public DeactivateUserCommandHandler(
            IUserDomain userDomain,
            IGlobalExceptionHandler globalExceptionHandler,
            IEndpointResponse<IDatabaseResult> endpointResponse)
        {
            _userDomain = userDomain;
            _globalExceptionHandler = globalExceptionHandler;
            _endpointResponse = endpointResponse;
        }
        #endregion

        #region Methods
        public async Task<IEndpointResponse<IDatabaseResult>> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _userDomain.GetUserByIdAsync(request.IdUser);
                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
                    var userDto = response.Details;
                    userDto.IsActive = false;
                    userDto.IsRegistrationConfirmed = false;
                    userDto.RegistrationConfirmedAt = DateTime.UtcNow;

                    var updateResult = await _userDomain.UpdateUserAsync(userDto);

                    if (updateResult.ResultStatus)
                    {
                        _endpointResponse.IsSuccess = true;
                        _endpointResponse.Message = "User deactivated successfully";
                    }
                    else
                    {
                        _endpointResponse.IsSuccess = false;
                        _endpointResponse.Message = "Failed to deactivate user";
                    }
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = "User not found";
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "DeactiveUserCommandHandler");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = ex.Message;
            }
            return _endpointResponse;
        }
        #endregion
    }
}
