using AutoMapper;
using MediatR;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Common.Responses;
using SharedKernel.Events.Auth;
using SharedKernel.Extensions.Event;
using SharedKernel.Extensions.Http;
using SharedKernel.Extensions.Routing;
using SharedKernel.Interfaces.Exceptions;
using SharedKernel.Interfaces.Response;
using UsersService.Application.DTO;
using UsersService.Domain.Interface;

namespace UsersService.Application.Queries.Handlers
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>>>
    {
        #region Properties
        private readonly IUserDomain _userDomain;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        private readonly IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>> _endpointResponse;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IHttpContextAccessor _contextAccessor;
        #endregion

        #region Constructor
        public GetUserByIdQueryHandler(
            IUserDomain userDomain,
            IMapper mapper,
            IApplicationExceptionHandler applicationExceptionHandler,
            IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>> endpointResponse,
            IEventPublisherService eventPublisherService,
            IHttpContextAccessor contextAccessor
            )
        {
            _userDomain = userDomain;
            _applicationExceptionHandler = applicationExceptionHandler;
            _endpointResponse = endpointResponse;
            _eventPublisherService = eventPublisherService;
            _contextAccessor = contextAccessor; 
        }
        #endregion

        #region Methods
        public async Task<IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _userDomain.GetUserByIdAsync(request.IdUser);
                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "Successful";

                    var additionalData = new
                    {
                        IdUser = request.IdUser,
                        FirstName = response.Details.FirstName,
                        LastName = response.Details.LastName,
                        Email = response.Details.Email,
                        CreatedAt = DateTime.Now
                    };

                    await _eventPublisherService.PublishEventAsync(
                        entityName: AuditEntityType.User.ToEntityName(),
                        operationType: AuditOperationType.Get.ToOperationType(),
                        success: true,
                        performedBy: _contextAccessor.GtePerformedBy(),
                        reason: response.ResultMessage,
                        additionalData: additionalData,
                        exchangeName: PublicationExchangeNames.User.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Get_Success.ToRoutingKey()
                        );
                }
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Handler, ActionType.Get);
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = ex.Message;

                var errorEvent = new RegisterErrorEvent
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace,
                };

                await _eventPublisherService.PublishEventAsync(
                    entityName: AuditEntityType.User.ToEntityName(),
                    operationType: AuditOperationType.Get.ToOperationType(),
                    success: false,
                    performedBy: _contextAccessor.GtePerformedBy(),
                    reason: ex.Message,
                    additionalData: errorEvent,
                    exchangeName: PublicationExchangeNames.User.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Get_Error.ToRoutingKey()
                    );
            }
            return _endpointResponse;
        }
        #endregion
    }
}
