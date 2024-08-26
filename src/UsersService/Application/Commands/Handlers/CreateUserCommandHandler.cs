using MediatR;
using SharedKernel.Interface;
using UsersService.Application.Dto;
using UsersService.Domain.Events;
using UsersService.Domain.Interface;
using UsersService.Infrastructure.Messaging;

namespace UsersService.Application.Commands.Handlers
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, IEndpointResponse<IDatabaseResult>>
    {

        #region Properties
        private readonly IUserDomain _userDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        private readonly EventBusRabbitMQ _eventBus;
        #endregion

        #region Constructor
        public CreateUserCommandHandler(
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
        public async Task<IEndpointResponse<IDatabaseResult>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // TODO:
            // create user
            // valid and user creation
            // publish event: UserCreatedEvent

            try
            {
                var user = new AddUserDTO
                {
                    IdRole = request.IdRole,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                };

                var response = await _userDomain.CreateUserAsync(user);
                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "User created successful";

                    // publish the event: UserCreatedEvent, through rabbitmq
                    var userCreatedEvent = new UserCreatedEvent
                    {
                        UserId = response.AffectedRecordId,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Email = request.Email
                    };

                    _eventBus.Publish("UserExchange", "UserCreated", userCreatedEvent);
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = response?.ResultMessage ?? "User not created";
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "CreateUserCommand.Handle");

                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = $"Error creating user: {ex.Message}";

                // publish the error event
                var userCreationFailedEvent = new UserCreationFailedEvent(
                    ex.Message,
                    request.FirstName + " " + request.LastName,
                    request.Email,
                    DateTime.UtcNow);

                _eventBus.Publish("UserExchange", "UserCreationFailed", userCreationFailedEvent);
            }
            return _endpointResponse;
        }
        #endregion
    }
}
