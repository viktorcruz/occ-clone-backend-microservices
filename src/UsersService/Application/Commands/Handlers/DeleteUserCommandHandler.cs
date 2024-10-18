using MediatR;
using SharedKernel.Interface;
using UsersService.Domain.Events;
using UsersService.Domain.Interface;
using UsersService.Infrastructure.Messaging;

namespace UsersService.Application.Commands.Handlers
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly IUserDomain _usersDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        private readonly EventBusRabbitMQ _eventBus;
        #endregion

        #region Constructor
        public DeleteUserCommandHandler(
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

        #region Methods
        public async Task<IEndpointResponse<IDatabaseResult>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _usersDomain.DeleteUserAsync(request.idUser);
                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "User deleted successful";


                    var additionalData = new
                    {
                        IdUser = request.idUser,
                        IsDeleted = true
                    };

                    var entityOperationEvent = new EntityOperationEvent(
                        entityName: "User",
                        operationType: "Delete",
                        success: true,
                        performedBy: "Admin",
                        reason: response.ResultStatus.ToString(),
                        additionalData: additionalData
                        );

                    _eventBus.Publish("UserExchange", "UserDeletedEvent", entityOperationEvent);

                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = response?.ResultMessage ?? "User not found";
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "DeleteUserCommand.Handle");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = $"Error deleting user: {ex.Message}";

                var failedEvent = new EntityOperationEvent(entityName: "User", operationType: "Delete", success: false, performedBy: "AdminUser");

                _eventBus.Publish("UserExchange", "UserDeletedFailed", failedEvent);

            }
            return _endpointResponse;
        }
        #endregion
    }
}
