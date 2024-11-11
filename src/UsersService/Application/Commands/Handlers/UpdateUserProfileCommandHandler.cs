using MediatR;
using SharedKernel.Common.Interfaces;
using SharedKernel.Interface;
using UsersService.Application.Dto;
using UsersService.Domain.Interface;

namespace UsersService.Application.Commands.Handlers
{
    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly IUserDomain _userDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        private readonly IEventBus _eventBus;
        private readonly IEntityOperationEventFactory _entityOperationEventFactory;
        #endregion

        #region Constructor
        public UpdateUserProfileCommandHandler(
            IUserDomain userDomain,
            IGlobalExceptionHandler globalExceptionHandler,
            IEndpointResponse<IDatabaseResult> endpointResponse,
            IEventBus eventBus,
            IEntityOperationEventFactory entityOperationEventFactory
            )
        {
            _userDomain = userDomain;
            _globalExceptionHandler = globalExceptionHandler;
            _endpointResponse = endpointResponse;
            _eventBus = eventBus;
            _entityOperationEventFactory = entityOperationEventFactory;
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

                    var eventInstance = _entityOperationEventFactory.CreateEvent(
                        entityName: "User",
                        operationType: "Update Profile",
                        success: true,
                        performedBy: "Admin",
                        reason: response.ResultStatus.ToString(),
                        additionalData: additionalData
                        );

                    _eventBus.Publish("user_exchange", "user.updated_profile", eventInstance);
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

                var failedEvent = _entityOperationEventFactory.CreateEvent(
                    entityName: "User",
                    operationType: "Update Profile",
                    success: true,
                    performedBy: "Admin",
                    reason: ex.Message,
                    additionalData: null
                    );
                _eventBus.Publish("user_exchange", "user.updated_profile.failed", failedEvent);
            }
            return _endpointResponse;
        }
        #endregion
    }
}
