using MediatR;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using UsersService.Application.Dto;
using UsersService.Domain.Events;
using UsersService.Domain.Interface;
using UsersService.Infrastructure.Messaging;

namespace UsersService.Application.Commands.Handlers
{
    public class ConfirmUserRegistrationCommandHandler : IRequestHandler<ConfirmUserRegistrationCommand, IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>>>
    {
        #region Properties
        private readonly IUserDomain _userDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>> _endpointResponse;
        private readonly EventBusRabbitMQ _eventBus;
        #endregion

        #region Constructor
        public ConfirmUserRegistrationCommandHandler(
            IUserDomain userDomain, 
            IGlobalExceptionHandler globalExceptionHandler, 
            IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>> endpointResponse,
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
        public async Task<IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>>> Handle(ConfirmUserRegistrationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _userDomain.GetUserByIdAsync(request.IdUser);
                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
                    var userDto = response.Details;
                    userDto.IsRegistrationConfirmed = true;
                    userDto.RegistrationConfirmedAt = DateTime.UtcNow;

                    var updateResult = await _userDomain.UpdateUserAsync(userDto);

                    if (updateResult.ResultStatus)
                    {
                        _endpointResponse.IsSuccess = true;
                        _endpointResponse.Message = "User registration cofirmed successfully";

                        var additionalData = new
                        {
                            IdUser = userDto.IdUser,
                            FirstName = userDto.FirstName,
                            LastName = userDto.LastName,
                            Email = userDto.Email
                        };

                        var entityOperationEvent = new EntityOperationEvent(
                            entityName: "User",
                            operationType: "Confirm",
                            success: true,
                            performedBy: "Admin",
                            reason: response.ResultStatus.ToString(),
                            additionalData: additionalData
                            );

                        _eventBus.Publish("UserExchange", "UserConfirmed", entityOperationEvent);
                    }
                    else
                    {
                        _endpointResponse.IsSuccess = false;
                        _endpointResponse.Message = "Failed to confirm user registration";
                    }
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = "User not found";
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "ConfirmUserRegistrationCommandHandler");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = ex.Message;

                var failedEvent = new EntityOperationEvent(entityName: "User", operationType: "Confirm", success: false, performedBy: "AdminUser");

                _eventBus.Publish("UserExchange", "UserConfirmedEvent", failedEvent);

            }
            return _endpointResponse;
        }
        #endregion
    }
}
