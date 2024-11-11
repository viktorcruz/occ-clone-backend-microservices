using SharedKernel.Common.Interfaces;
using SharedKernel.Interface;
using UsersService.Application.Dto;
using UsersService.Domain.Interface;
using UsersService.Infrastructure.Interface;

namespace UsersService.Events.Handlers
{
    public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        private readonly IEventBus _eventBus;
        private readonly IEntityOperationEventFactory _entityOperationEventFactory;

        public UserCreatedEventHandler(
            IUserRepository userRepository,
            IGlobalExceptionHandler globalExceptionHandler,
            IEndpointResponse<IDatabaseResult> endpointResponse,
            IEventBus eventBus,
            IEntityOperationEventFactory entityOperationEventFactory
            )
        {
            _userRepository = userRepository;
            _globalExceptionHandler = globalExceptionHandler;
            _endpointResponse = endpointResponse;
            _eventBus = eventBus;
            _entityOperationEventFactory = entityOperationEventFactory;
        }

        public async Task Handle(UserCreatedEvent @event)
        {
            try
            {
                var dto = new AddUserDTO
                {
                    IdRole = @event.IdRole,
                    FirstName = @event.FirstName,
                    LastName = @event.LastName,
                    Email = @event.Email,
                };

                var result = await _userRepository.CreateUserAsync(dto);

                //if (result != null && result.ResultStatus)
                //{
                //    // Crear y publicar el evento de éxito
                //    var userCreationSucceededEvent = new UserCreationSucceededEvent(result.AffectedRecordId, "User successfully created.");

                //    _eventBus.Publish("user_exchange", "UserEventRoutingKey", userCreationSucceededEvent);


                //    // Opcional: crear un registro de operación para auditoría
                //    var eventInstance = _entityOperationEventFactory.CreateEvent(
                //        entityName: "User",
                //        operationType: "Create",
                //        success: true,
                //        performedBy: "Admin",
                //        reason: "User created successfully",
                //        additionalData: null
                //    );

                //    _eventBus.Publish("user_exchange", "UserEventRoutingKey", eventInstance);
                //}
                //else
                //{
                //    // Publicar evento de fallo
                //    var userCreationFailedEvent = new UserCreationFailedEvent(result.AffectedRecordId, "Error creating user");
                //    _eventBus.Publish("user_exchange", "UserEventRoutingKey", userCreationFailedEvent);
                //}
                if (result != null && result.ResultStatus)
                {
                    var userCreationSucceededEvent = new UserCreationSucceededEvent(result.AffectedRecordId, "User successfully created.");
                    _eventBus.Publish("user_exchange", "user.created.success", userCreationSucceededEvent);

                    var eventInstance = _entityOperationEventFactory.CreateEvent(
                        entityName: "User",
                        operationType: "Create",
                        success: true,
                        performedBy: "Admin",
                        reason: "User created successfully",
                        additionalData: null
                    );

                    _eventBus.Publish("user_exchange", "audit.user.create.success", eventInstance);
                }
                else
                {
                    var userCreationFailedEvent = new UserCreationFailedEvent(result.AffectedRecordId, "Error creating user");
                    _eventBus.Publish("user_exchange", "user.created.failure", userCreationFailedEvent);
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción y publicar evento de fallo
                _globalExceptionHandler.HandleGenericException<string>(ex, "UserCreatedEventHandler.Handle");
                var failedEvent = new UserCreationFailedEvent(0, ex.Message);
                _eventBus.Publish("user_exchange", "UserEventRoutingKey", failedEvent);
            }
        }
    }
}
