//using UsersService.Application.Commands;
//using UsersService.Infrastructure.Interface;

//namespace UsersService.Saga
//{
//    public class UserSagaHandler
//    {
//        private readonly IUserRepository _userRepository;
//        private readonly IPublicationService _publicationService;
//        private readonly ISearchJobsService _searchJobsService;
//        private readonly UserSagaContext _sagaState;
//        private readonly CompensationActions _compensationActions;

//        public UserSagaHandler(
//            IUserRepository userRepository,
//            IPublicationService publicationService,
//            ISearchJobsService searchJobsService,
//            CompensationActions compensationActions
//        )
//        {
//            _userRepository = userRepository;
//            _publicationService = publicationService;
//            _searchJobsService = searchJobsService;
//            _compensationActions = compensationActions;
//            _sagaState = new UserSagaContext();
//        }

//        public async Task Handle(CreateUserCommand command)
//        {
//            try
//            {
//                if (_sagaState.CurrentState == SagaState.NotStarted)
//                {
//                    // Paso 1: Crear Usuario
//                    var dto = new Application.Dto.AddUserDTO
//                    {
//                        IdRole = command.IdRole,
//                        FirstName = command.FirstName,
//                        LastName = command.LastName,
//                        Email = command.Email,
//                    };
//                    var result = await _userRepository.CreateUserAsync(dto);

//                    if (result != null && result.ResultStatus)
//                    {
//                        _sagaState.MarkUserCreated(result.AffectedRecordId);
//                    }
//                    else
//                    {
//                        throw new Exception("User creation failed");
//                    }
//                }

//                if (_sagaState.CurrentState == SagaState.UserCreated)
//                {
//                    // Paso 2: Crear Publicación en PublicationService
//                    var publicationDto = new Application.Dto.PublicationDTO
//                    {
//                        IdUser = _sagaState.IdUser,
//                        Title = command.Title,
//                        Description = command.Description,
//                        ExpirationDate = command.ExpirationDate,
//                        Status = command.Status,
//                        Salary = command.Salary,
//                        Location = command.Location,
//                        Company = command.Company
//                    };

//                    var publicationId = await _publicationService.CreatePublicationAsync(publicationDto);
//                    if (publicationId > 0)
//                    {
//                        _sagaState.MarkPublicationCreated(publicationId);
//                    }
//                    else
//                    {
//                        throw new Exception("Publication creation failed");
//                    }
//                }

//                if (_sagaState.CurrentState == SagaState.PublicationCreated)
//                {
//                    // Paso 3: Actualizar el índice de búsqueda en SearchJobsService
//                    var updateResult = await _searchJobsService.UpdateJobSearchAsync(_sagaState.IdUser, _sagaState.IdPublication);
//                    if (updateResult)
//                    {
//                        _sagaState.MarkJobSearchUpdated();
//                    }
//                    else
//                    {
//                        throw new Exception("Job search update failed");
//                    }
//                }

//                // Completar la saga si todos los pasos fueron exitosos
//                _sagaState.CompleteSaga();
//            }
//            catch (Exception ex)
//            {
//                // Activar la lógica de compensación en caso de error
//                await _compensationActions.CompensateCreateUserAsync(_sagaState);
//                throw;
//            }
//        }
//    }

//}
