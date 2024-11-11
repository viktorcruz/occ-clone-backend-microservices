//using MediatR;
//using SharedKernel.Common.Interfaces;
//using SharedKernel.Interface;
//using UsersService.Application.Dto;
//using UsersService.Domain.Interface;

//namespace UsersService.Application.Commands.Handlers
//{
//    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, IEndpointResponse<IDatabaseResult>>
//    {

//        #region Properties
//        private readonly IUserDomain _userDomain;
//        private readonly IGlobalExceptionHandler _globalExceptionHandler;
//        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
//        private readonly IEventBus _eventBus;
//        private readonly IEntityOperationEventFactory _entityOperationEventFactory;
//        #endregion

//        #region Constructor
//        public CreateUserCommandHandler(
//            IUserDomain userDomain,
//            IGlobalExceptionHandler globalExceptionHandler,
//            IEndpointResponse<IDatabaseResult> endpointResponse,
//            IEventBus eventBus,
//            IEntityOperationEventFactory entityOperationEventFactory
//            )
//        {
//            _userDomain = userDomain;
//            _globalExceptionHandler = globalExceptionHandler;
//            _endpointResponse = endpointResponse;
//            _eventBus = eventBus;
//            _entityOperationEventFactory = entityOperationEventFactory;
//        }
//        #endregion

//        #region Methods
//        public async Task<IEndpointResponse<IDatabaseResult>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var user = new AddUserDTO
//                {
//                    IdRole = request.IdRole,
//                    FirstName = request.FirstName,
//                    LastName = request.LastName,
//                    Email = request.Email,
//                };

//                var response = await _userDomain.CreateUserAsync(user);
//                _endpointResponse.Result = response;

//                if (response != null && response.ResultStatus)
//                {
//                    _endpointResponse.IsSuccess = true;
//                    _endpointResponse.Message = "User created successful";

//                    var additionalData = new
//                    {
//                        FirstName = request.FirstName,
//                        LastName = request.LastName,
//                        Email = request.Email,
//                    };

//                    var eventInstance = _entityOperationEventFactory.CreateEvent(
//                        entityName: "User",
//                        operationType: "Create",
//                        success: true,
//                        performedBy: "Admin",
//                        reason: response.ResultStatus.ToString(),
//                        additionalData: additionalData
//                        );

//                    _eventBus.Publish("user_exchange", "user.created", eventInstance);
//                }
//                else
//                {
//                    _endpointResponse.IsSuccess = false;
//                    _endpointResponse.Message = response?.ResultMessage ?? "User not created";
//                }
//            }
//            catch (Exception ex)
//            {
//                _globalExceptionHandler.HandleGenericException<string>(ex, "CreateUserCommand.Handle");

//                _endpointResponse.IsSuccess = false;
//                _endpointResponse.Message = $"Error creating user: {ex.Message}";

//                var failedEvent = _entityOperationEventFactory.CreateEvent(
//                    entityName: "User",
//                    operationType: "Create",
//                    success: true,
//                    performedBy: "Admin",
//                    reason: ex.Message,
//                    additionalData: null
//                    );
//                _eventBus.Publish("user_exchange", "user.created.failed", failedEvent);
//            }
//            return _endpointResponse;
//        }
//        #endregion
//    }
//}
