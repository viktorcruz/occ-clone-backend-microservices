using SharedKernel.Common.Events;
using SharedKernel.Common.Extensions;
using SharedKernel.Common.Interfaces;
using UsersService.Infrastructure.Interface;

namespace UsersService.Saga
{
    public class UserSagaHandler : IEventHandler<RegisterSuccessEvent>, IEventHandler<RegisterErrorEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly UserSagaContext _sagaState;
        private readonly CompensationActions _compensationActions;

        public UserSagaHandler(
            IUserRepository userRepository,
            IEventPublisherService eventPublisherService,
            CompensationActions compensationActions
        )
        {
            _userRepository = userRepository;
            _eventPublisherService = eventPublisherService;
            _compensationActions = compensationActions;
            _sagaState = new UserSagaContext();
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
                        entityName: "",
                        operationType: "",
                        success: true,
                        performedBy: "Admin",
                        reason: "",
                        additionalData: null,
                        exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Created.ToRoutingKey()
                        );

                    _sagaState.TransitionToState(SagaState.UserCreated);
                    //var publicationId = await _publicationServcie.CreatePublicationAsync(publicationDto);
                    //if(publicationId > 0)
                    //{
                    //    _sagaState.MarkPublicationCreated(publicationId);
                    //}
                    //else
                    //{
                    //    throw new Exception("Publication creation failed");
                    //}
                    //var dto = new Application.Dto.AddUserDTO
                    //{
                    //    IdRole = command.IdRole,
                    //    FirstName = command.FirstName,
                    //    LastName = command.LastName,
                    //    Email = command.Email,
                    //};
                    //var result = await _userRepository.CreateUserAsync(dto);

                    //if (result != null && result.ResultStatus)
                    //{
                    //    _sagaState.MarkUserCreated(result.AffectedRecordId);
                    //}
                    //else
                    //{
                    //    throw new Exception("User creation failed");
                    //}
                }

                //if (_sagaState.CurrentState == SagaState.PublicationCreated)
                //{
                //    // Paso 3: actualizar el indice de busqueda en searchJobsService
                //    //var publicationDto = new Application.Dto.PublicationDTO
                //    //{
                //    //    IdUser = _sagaState.IdUser,
                //    //    Title = command.Title,
                //    //    Description = command.Description,
                //    //    ExpirationDate = command.ExpirationDate,
                //    //    Status = command.Status,
                //    //    Salary = command.Salary,
                //    //    Location = command.Location,
                //    //    Company = command.Company
                //    //};

                //    var updateResult = await _searchJobsService.UpdateJobSearchAsync(_sagaState.IdUser, _sagaState.IdPublication);
                //    if (updateResult)
                //    {
                //        _sagaState.MarkJobSearchUpdated();
                //    }
                //    else
                //    {
                //        throw new Exception("Job search update failed");
                //    }
                //}

                //if (_sagaState.CurrentState == SagaState.PublicationCreated)
                //{
                //    // Paso 3: Actualizar el índice de búsqueda en SearchJobsService
                //    var updateResult = await _searchJobsService.UpdateJobSearchAsync(_sagaState.IdUser, _sagaState.IdPublication);
                //    if (updateResult)
                //    {
                //        _sagaState.MarkJobSearchUpdated();
                //    }
                //    else
                //    {
                //        throw new Exception("Job search update failed");
                //    }
                //}

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
                    entityName: "",
                    operationType: "",
                    success: false,
                    performedBy: "Admin",
                    reason: "",
                    additionalData: null,
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
