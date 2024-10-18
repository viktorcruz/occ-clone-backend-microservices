using MediatR;
using SharedKernel.Interface;
using UsersService.Domain.Events;
using UsersService.Domain.Interface;
using UsersService.Infrastructure.Messaging;

namespace UsersService.Application.Commands.Handlers
{
    public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly IUserDomain _userDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        private readonly EventBusRabbitMQ _eventBus;
        #endregion

        #region Constructor
        public ActivateUserCommandHandler(
            IUserDomain userDomain,
            IGlobalExceptionHandler globalExceptionHandler,
            IEndpointResponse<IDatabaseResult> endpointResponse,
            EventBusRabbitMQ eventBus
            )
        {
            _userDomain = userDomain;
            _globalExceptionHandler = globalExceptionHandler;   
            _endpointResponse = endpointResponse;
            _eventBus = eventBus;
        }
        #endregion

        #region Methods
        public async Task<IEndpointResponse<IDatabaseResult>> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _userDomain.GetUserByIdAsync(request.IdUser);
                _endpointResponse.Result = response;

                if(response != null && response.ResultStatus)
                {
                    var userDto = response.Details;
                    userDto.IsActive = true;

                    var updateRestult = await _userDomain.UpdateUserAsync(userDto);

                    if(updateRestult != null)
                    {
                        _endpointResponse.IsSuccess = true;
                        _endpointResponse.Message = "User sucessfully activated";

                        var additionalData = new
                        {
                            IdUser = userDto.IdUser,
                            FirstName = userDto.FirstName,
                            LastName = userDto.LastName,
                            Email = userDto.Email,
                            IsActivated = true,
                        };

                        var entityOperationEvent = new EntityOperationEvent(
                            entityName: "User",
                            operationType: "Activate",
                            success: true,
                            performedBy: "Admin",
                            reason: response.ResultStatus.ToString(),
                            additionalData: additionalData
                            );

                        _eventBus.Publish("UserExchange", "UserActivatedEvent", entityOperationEvent);
                    }
                    else
                    {
                        _endpointResponse.IsSuccess = false;
                        _endpointResponse.Message = "Filed to activate user";
                    }
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = "User not found";
                }
            }
            catch(Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "ActivateUserCommandHandler");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = ex.Message;

                var failedEvent = new EntityOperationEvent(entityName: "User", operationType: "Activate", success: false, performedBy: "AdminUser");

                _eventBus.Publish("UserExchange", "UserDeactivatedEvent", failedEvent);
            }
            return _endpointResponse;
        }
        #endregion
    }
}
