using MediatR;
using SharedKernel.Interface;
using UsersService.Application.Dto;
using UsersService.Domain.Events;
using UsersService.Domain.Interface;
using UsersService.Infrastructure.Messaging;

namespace UsersService.Application.Commands.Handlers
{
    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly IUserDomain _userDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        private readonly EventBusRabbitMQ _eventBus;
        #endregion

        #region Constructor
        public UpdateUserProfileCommandHandler(
            IUserDomain userDomain,
            IGlobalExceptionHandler globalExceptionHandler,
            IEndpointResponse<IDatabaseResult> endpointResponse,
            EventBusRabbitMQ eventBus)
        {
            _userDomain = userDomain;
            _globalExceptionHandler = globalExceptionHandler;
            _endpointResponse = endpointResponse;
            _eventBus = eventBus;
        }
        #endregion

        #region Methods
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

                    var additionalData = new
                    {
                        IdUser = userProfile.IdUser,
                        FirstName = userProfile.FirstName,
                        LastName = userProfile.LastName,
                        Email = userProfile.Email
                    };

                    var entityOperationEvent = new EntityOperationEvent(
                        entityName: "User",
                        operationType: "Update Profile",
                        success: true,
                        performedBy: "Admin",
                        reason: response.ResultStatus.ToString(),
                        additionalData: additionalData
                        );

                    _eventBus.Publish("UserExchange", "UserUpdatedProfileEvent", entityOperationEvent);

                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = response?.ResultMessage ?? "Profile not found";
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "UpdateUserProfileCommandHandler");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = $"Error updating profile: {ex.Message}";

                var failedEvent = new EntityOperationEvent(entityName: "User", operationType: "UpdateProfile", success: false, performedBy: "AdminUser");

                _eventBus.Publish("UserExchange", "UserUpdateProfileFailed", failedEvent);
            }
            return _endpointResponse;
        }
        #endregion
    }
}
