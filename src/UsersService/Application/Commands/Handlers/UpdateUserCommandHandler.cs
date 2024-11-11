using MediatR;
using SharedKernel.Common.Interfaces;
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
        private readonly IEventBus _eventBus;
        private readonly IEntityOperationEventFactory _entityOperationEventFactory;
        #endregion

        #region Constructor
        public UpdateUserCommandHandler(
            IUserDomain usersDomain,
            IGlobalExceptionHandler globalExceptionHandler,
            IEndpointResponse<IDatabaseResult> endpointResponse,
            IEventBus eventBus,
            IEntityOperationEventFactory entityOperationEventFactory
            )
        {
            _usersDomain = usersDomain;
            _globalExceptionHandler = globalExceptionHandler;
            _endpointResponse = endpointResponse;
            _eventBus = eventBus;
            _entityOperationEventFactory = entityOperationEventFactory;
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

                    var additionalData = new
                    {
                        IdUser = user.IdUser,
                        IdRole = user.IdRole,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        IsActive = user.IsActive,
                    };

                    var eventInstance = _entityOperationEventFactory.CreateEvent(
                        entityName: "User",
                        operationType: "Update",
                        success: true,
                        performedBy: "Admin",
                        reason: response.ResultStatus.ToString(),
                        additionalData: additionalData
                        );

                    _eventBus.Publish("user_exchange", "user.updated", eventInstance);
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = response?.ResultMessage ?? "User not found";
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "UpdateUserCommandHandler");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = $"Error updating user: {ex.Message}";

                var failedEvent = _entityOperationEventFactory.CreateEvent(
                    entityName: "User",
                    operationType: "UPDATE",
                    success: true,
                    performedBy: "Admin",
                    reason: ex.Message,
                    additionalData: null
                    );
                _eventBus.Publish("user_exchange", "user.updated.failed", failedEvent);
            }
            return _endpointResponse;
        }
    }
}
