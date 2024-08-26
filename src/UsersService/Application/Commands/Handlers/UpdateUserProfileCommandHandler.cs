using MediatR;
using SharedKernel.Interface;
using UsersService.Application.Dto;
using UsersService.Domain.Interface;

namespace UsersService.Application.Commands.Handlers
{
    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, IEndpointResponse<IDatabaseResult>>
    {
        private readonly IUserDomain _userDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;

        public UpdateUserProfileCommandHandler(
            IUserDomain userDomain,
            IGlobalExceptionHandler globalExceptionHandler,
            IEndpointResponse<IDatabaseResult> endpointResponse)
        {
            _userDomain = userDomain;
            _globalExceptionHandler = globalExceptionHandler;
            _endpointResponse = endpointResponse;
        }

        public async Task<IEndpointResponse<IDatabaseResult>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userProfile = new UserProfileDTO
                {
                    IdUser = request.IdUser,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                };

                var response = await _userDomain.UpdateUserProfileAsync(userProfile);
                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "Profile updated successful";
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = response?.ResultMessage ?? "Profile not found";
                }

                return _endpointResponse;
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "UpdateUserProfileCommandHandler");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = $"Error updating profile: {ex.Message}";
                return _endpointResponse;
            }
        }
    }
}
