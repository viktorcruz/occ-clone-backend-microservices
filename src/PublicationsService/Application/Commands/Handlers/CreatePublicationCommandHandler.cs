using MediatR;
using PublicationsService.Domain.Interface;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Events.Auth;
using SharedKernel.Extensions.Event;
using SharedKernel.Extensions.Http;
using SharedKernel.Extensions.Routing;
using SharedKernel.Interfaces.Exceptions;
using SharedKernel.Interfaces.Response;

namespace PublicationsService.Aplication.Commands.Handlers
{
    public class CreatePublicationCommandHandler : IRequestHandler<CreatePublicationCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly IPublicationDomain _publicationDomain;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        private readonly IHttpContextAccessor _contextAccessor;
        #endregion

        #region Constructor
        public CreatePublicationCommandHandler(
            IPublicationDomain publicationDomain,
            IEventPublisherService eventPublisherService,
            IApplicationExceptionHandler applicationExceptionHandler,
            IEndpointResponse<IDatabaseResult> endpointResponse,
            IHttpContextAccessor contextAccessor
            )
        {
            _publicationDomain = publicationDomain;
            _eventPublisherService = eventPublisherService;
            _applicationExceptionHandler = applicationExceptionHandler;
            _endpointResponse = endpointResponse;
            _contextAccessor = contextAccessor;
        }
        #endregion

        #region Methods
        public async Task<IEndpointResponse<IDatabaseResult>> Handle(CreatePublicationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _publicationDomain.CreatePublicationAsync(request);

                _endpointResponse.Result = response;

                if (response == null || !response.ResultStatus)
                {
                    throw new Exception("User not found");
                }

                _endpointResponse.IsSuccess = true;
                _endpointResponse.Message = "Publication created successful";

                var additionalData = new
                {
                    IdUser = request.IdUser,
                    IdRole = request.IdRole,
                    Title = request.Title,
                    Description = request.Description,
                    ExpirationDate = request.ExpirationDate,
                    Status = request.Status,
                    Salary = request.Salary,
                    Location = request.Location,
                };

                await _eventPublisherService.PublishEventAsync(
                    entityName: AuditEntityType.Publication.ToEntityName(),
                    operationType: AuditOperationType.Create.ToOperationType(),
                    success: true,
                    performedBy: _contextAccessor.GtePerformedBy(),
                    reason: response.ResultMessage,
                    additionalData: additionalData,
                    exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Created.ToRoutingKey()
                );
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Repository, ActionType.Query);
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = $"Error creating publication: {ex.Message}";

                var registerErrorEvent = new RegisterErrorEvent
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };

                await _eventPublisherService.PublishEventAsync(
                    entityName: AuditEntityType.Publication.ToEntityName(),
                    operationType: AuditOperationType.Create.ToOperationType(),
                    success: false,
                    performedBy: _contextAccessor.GtePerformedBy(),
                    reason: ex.Message,
                    additionalData: registerErrorEvent,
                    exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Create_Error.ToRoutingKey()
                    );
            }

            return _endpointResponse;
        }
        #endregion
    }
}
