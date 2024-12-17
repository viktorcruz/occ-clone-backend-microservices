using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Events.Auth;
using SharedKernel.Events.Publication;
using SharedKernel.Extensions.Routing;
using UsersService.Saga.Interfaces;

namespace UsersService.Saga
{
    public class UserSagaHandler : IUserSagaHandler, IEventHandler<RegisterSuccessEvent>, IEventHandler<RegisterErrorEvent>
    {
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IUserSagaContext _sagaState;
        private readonly ICompensationActions _compensationActions;

        public UserSagaHandler(
            IEventPublisherService eventPublisherService,
            IUserSagaContext userSagaContext,
            ICompensationActions compensationActions
        )
        {
            _eventPublisherService = eventPublisherService;
            _compensationActions = compensationActions;
            _sagaState = userSagaContext;
        }

        public async Task Handle(RegisterSuccessEvent registerEvent)
        {
            try
            {
                if (_sagaState.CurrentState == SagaState.NotStarted)
                {
                    // Paso 1: usuario registrado, guardar el ID en el estado de la saga
                    _sagaState.MarkUserCreated(registerEvent.IdUser);

                    // Paso 2: crear publicacion en publicationService
                    var publicatoinCreatedEvent = new PublicationCreatedEvent
                    {
                        IdUser = registerEvent.IdUser,
                        Title = registerEvent.Title,
                        Description = registerEvent.Description,
                        ExpirationDate = registerEvent.ExpirationDate,
                        Status = registerEvent.Status,
                        Salary = registerEvent.Salary,
                        Location = registerEvent.Location,
                        Company = registerEvent.Company
                    };

                    await _eventPublisherService.PublishEventAsync(
                        entityName: "User",
                        operationType: "SAGA",
                        success: true,
                        performedBy: "SYSTEM",
                        reason: string.Empty,
                        additionalData: publicatoinCreatedEvent,
                        exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Created.ToRoutingKey()
                        );

                    _sagaState.TransitionToState(SagaState.UserCreated);
                }

                // Completar la saga si todos los pasos fueron exitosos
                _sagaState.CompleteSaga();
            }
            catch (Exception ex)
            {
                // Activar la lógica de compensación en caso de error
                await _compensationActions.CompensateCreateUserAsync(_sagaState);
                var errorEvent = new RegisterErrorEvent
                {
                    IdUser = registerEvent.IdUser,
                    ErrorMessage = ex.Message,
                };
                await _eventPublisherService.PublishEventAsync(
                    entityName: "User",
                    operationType: "SAGA",
                    success: false,
                    performedBy: "SYSTEM",
                    reason: ex.Message,
                    additionalData: errorEvent,
                    exchangeName: PublicationExchangeNames.User.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Register_Error.ToRoutingKey()
                    );
            }
        }

        public async Task Handle(RegisterErrorEvent errorEvent)
        {
            Console.WriteLine($"Handling RegisterErrorEvent: {errorEvent.ErrorMessage}");

            await _compensationActions.CompensateCreateUserAsync(_sagaState);

            Console.WriteLine("Compensation actions executed");
        }
    }

}
