using MediatR;
using SharedKernel.Common.Interfaces;
using SharedKernel.Interface;
using UsersService.Domain.Interface;

namespace UsersService.Application.Commands.Handlers
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly IUserDomain _usersDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        private readonly IEventBus _eventBus;
        private readonly IEntityOperationEventFactory _entityOperationEventFactory;
        #endregion

        #region Constructor
        public DeleteUserCommandHandler(
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

                    var eventInstance = _entityOperationEventFactory.CreateEvent(
                        entityName: "User",
                        operationType: "Delete",
                        success: true,
                        performedBy: "Admin",
                        reason: response.ResultStatus.ToString(),
                        additionalData: additionalData
                        );

                    _eventBus.Publish("user_exchange", "user.deleted", eventInstance);
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

                var failedEvent = _entityOperationEventFactory.CreateEvent(
                    entityName: "User",
                    operationType: "Delete",
                    success: true,
                    performedBy: "Admin",
                    reason: ex.Message,
                    additionalData: null
                    );
                _eventBus.Publish("user_exchange", "user.deleted.failed", failedEvent);

            }
            return _endpointResponse;
        }
        #endregion
    }
}
