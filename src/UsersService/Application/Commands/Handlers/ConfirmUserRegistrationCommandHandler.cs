using MediatR;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using UsersService.Application.Dto;
using UsersService.Domain.Interface;

namespace UsersService.Application.Commands.Handlers
{
    public class ConfirmUserRegistrationCommandHandler : IRequestHandler<ConfirmUserRegistrationCommand, IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>>>
    {
        #region Properties
        private readonly IUserDomain _userDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>> _endpointResponse;
        #endregion

        #region Constructor
        public ConfirmUserRegistrationCommandHandler(IUserDomain userDomain, IGlobalExceptionHandler globalExceptionHandler, IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>> endpointResponse)
        {
            _userDomain = userDomain;
            _globalExceptionHandler = globalExceptionHandler;
            _endpointResponse = endpointResponse;
        }
        #endregion

        #region Methods
        public async Task<IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>>> Handle(ConfirmUserRegistrationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _userDomain.GetUserByIdAsync(request.IdUser);
                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
                    var userDto = response.Details;
                    userDto.IsRegistrationConfirmed = true;
                    userDto.RegistrationConfirmedAt = DateTime.UtcNow;

                    var updateResult = await _userDomain.UpdateUserAsync(userDto);

                    if (updateResult.ResultStatus)
                    {
                        _endpointResponse.IsSuccess = true;
                        _endpointResponse.Message = "User registration cofirmed successfully";
                    }
                    else
                    {
                        _endpointResponse.IsSuccess = false;
                        _endpointResponse.Message = "Failed to confirm user registration";
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
                _globalExceptionHandler.HandleGenericException<string>(ex, "ConfirmUserRegistrationCommandHandler");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = ex.Message;
            }
            return _endpointResponse;
        }
        #endregion
    }
}
