using MediatR;
using SharedKernel.Interface;
using UsersService.Application.Dto;
using UsersService.Domain.Events;
using UsersService.Domain.Interface;
using UsersService.Infrastructure.Messaging;

namespace UsersService.Application.Commands.Handlers
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly IUserDomain _usersDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        private readonly EventBusRabbitMQ _eventBus;
        #endregion

        #region Constructor
        public UpdateUserCommandHandler(
            IUserDomain usersDomain,
            IGlobalExceptionHandler globalExceptionHandler,
            IEndpointResponse<IDatabaseResult> endpointResponse,
            EventBusRabbitMQ eventBus
            )
        {
            _usersDomain = usersDomain;
            _globalExceptionHandler = globalExceptionHandler;
            _endpointResponse = endpointResponse;
            _eventBus = eventBus;
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

                    var entityOperationEvent = new EntityOperationEvent(
                        entityName: "User",
                        operationType: "Update",
                        success: true,
                        performedBy: "Admin",
                        reason: response.ResultStatus.ToString(),
                        additionalData: additionalData
                        );

                    _eventBus.Publish("UserExchange", "UserUpdatedEvent", entityOperationEvent);
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

                var failedEvent = new EntityOperationEvent(entityName: "User", operationType: "Update", success: false, performedBy: "AdminUser");

                _eventBus.Publish("UserExchange", "UserUpdateFailed", failedEvent);
            }
            return _endpointResponse;
        }
    }
}
