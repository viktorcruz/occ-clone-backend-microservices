using MediatR;
using SharedKernel.Interface;
using UsersService.Domain.Interface;

namespace UsersService.Application.Commands.Handlers
{
    public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly IUserDomain _userDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        #endregion

        #region Constructor
        public ActivateUserCommandHandler(
            IUserDomain userDomain,
            IGlobalExceptionHandler globalExceptionHandler,
            IEndpointResponse<IDatabaseResult> endpointResponse
            )
        {
            _userDomain = userDomain;
            _globalExceptionHandler = globalExceptionHandler;   
            _endpointResponse = endpointResponse;
        }
        #endregion

        #region Methods
        public async Task<IEndpointResponse<IDatabaseResult>> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _userDomain.GetUserByIdAsync(request.IdUser);
                _endpointResponse.Result = response;

                if(response != null && response.ResultStatus)
                {
                    var userDto = response.Details;
                    userDto.IsActive = true;
                    //userDto.IsRegistrationConfirmed = false;
                    //userDto.RegistrationConfirmedAt = DateTime.UtcNow;

                    var updateRestult = await _userDomain.UpdateUserAsync(userDto);

                    if(updateRestult != null)
                    {
                        _endpointResponse.IsSuccess = true;
                        _endpointResponse.Message = "User sucessfully activated";
                    }
                    else
                    {
                        _endpointResponse.IsSuccess = false;
                        _endpointResponse.Message = "Filed to activate user";
                    }
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = "User not found";
                }
            }
            catch(Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "ActivateUserCommandHandler");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = ex.Message;
            }
            return _endpointResponse;
        }
        #endregion
    }
}
