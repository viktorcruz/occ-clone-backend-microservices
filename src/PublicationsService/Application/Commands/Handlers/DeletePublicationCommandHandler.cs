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
    public class DeletePublicationCommandHandler : IRequestHandler<DeletePublicationCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly IPublicationDomain _publicationDomain;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        private readonly IHttpContextAccessor _contextAccessor;
        #endregion

        #region Constructor
        public DeletePublicationCommandHandler(
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
        public async Task<IEndpointResponse<IDatabaseResult>> Handle(DeletePublicationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _publicationDomain.DeletePublicationByIdAsync(request.IdPublication);
                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "Publication deleted successful";

                    var additionalData = new
                    {
                        IdPublication = request.IdPublication,
                        IsDeleted = true
                    };

                    await _eventPublisherService.PublishEventAsync(
                        entityName: AuditEntityType.Publication.ToEntityName(),
                        operationType: AuditOperationType.Delete.ToOperationType(),
                        success: true,
                        performedBy: _contextAccessor.GtePerformedBy(),
                        reason: response.ResultMessage,
                        additionalData: additionalData,
                        exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Deleted.ToRoutingKey()
                        );
                }
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Handler, ActionType.Delete);
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = $"Error deleting publication: {ex.Message}";
                var registerErrorEvent = new RegisterErrorEvent
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };
                await _eventPublisherService.PublishEventAsync(
                        entityName: AuditEntityType.Publication.ToEntityName(),
                        operationType: AuditOperationType.Delete.ToOperationType(),
                        success: false,
                        performedBy: _contextAccessor.GtePerformedBy(),
                        reason: ex.Message,
                        additionalData: registerErrorEvent,
                        exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Delete_Error.ToRoutingKey()
                        );
            }
            return _endpointResponse;
        }
        #endregion
    }
}
