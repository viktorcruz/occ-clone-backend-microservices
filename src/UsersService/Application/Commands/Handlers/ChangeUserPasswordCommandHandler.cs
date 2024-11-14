using MediatR;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Common.Events;
using SharedKernel.Common.Extensions;
using SharedKernel.Common.Interfaces;
using SharedKernel.Interface;
using SharedKernel.Security.Extensions;
using UsersService.Domain.Interface;

namespace UsersService.Application.Commands.Handlers
{
    public class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly IUserDomain _userDomain;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        private readonly IEventPublisherService _eventPublisherService;
        #endregion

        #region Constructor
        public ChangeUserPasswordCommandHandler(
            IUserDomain userDomain,
            IApplicationExceptionHandler applicationExceptionHandler,
            IEndpointResponse<IDatabaseResult> endpointResponse,
            IEventPublisherService eventPublisherService
            )
        {
            _userDomain = userDomain;
            _applicationExceptionHandler = applicationExceptionHandler;
            _eventPublisherService = eventPublisherService;
            _endpointResponse = endpointResponse;
        }
        #endregion

        #region Methods
        public async Task<IEndpointResponse<IDatabaseResult>> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingUser = await _userDomain.GetByEmailAsync(request.Email);
                if(existingUser.Details == null || !existingUser.ResultStatus)
                {
                    throw new Exception("Invalid Credentials");
                }
                if(!Convert.ToBoolean(existingUser.Details?.IsActive) && !Convert.ToBoolean(existingUser.Details?.IsRegistrationConfirmed))
                {
                    throw new Exception("User not yet activated");
                }

                bool passHashMatch = PasswordHashingExtensions.VerifyPassword(request.CurrentPassword, existingUser.Details.PasswordHash);

                if(!passHashMatch)
                {
                    throw new Exception("Invalid Credentials");
                }

                string hashedPassword = PasswordHashingExtensions.HashPassword(request.NewPassword);
                bool isMatch = PasswordHashingExtensions.VerifyPassword(request.NewPassword, hashedPassword);

                if(!isMatch)
                {
                    throw new Exception("");
                }

                var response = await _userDomain.ChangeUserPasswordAsync(existingUser.Details.IdUser, hashedPassword, existingUser.Details.Email);

                var newPassword = new
                {
                    IdUser = existingUser.Details?.IdUser,
                    Email = existingUser.Details?.Email,
                    Message = "Successfully updated"
                };

                await _eventPublisherService.PublishEventAsync(
                    entityName: "User",
                    operationType: "CHANGE_PASSWORD",
                    success: true,
                    performedBy: "Admin",
                    additionalData: newPassword,
                    exchangeName: PublicationExchangeNames.User.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Update_Success.ToRoutingKey()
                );

                _endpointResponse.IsSuccess = true;
                _endpointResponse.Message = "Successfully updated";
                _endpointResponse.Result = response;

                return _endpointResponse;
            }
            catch(Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Handler, ActionType.Update);
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = $"Error when cahnging password: {ex.Message}";
                var errorEvent = new RegisterErrorEvent
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };
            }
            return _endpointResponse;
        }

        #endregion
    }
}
