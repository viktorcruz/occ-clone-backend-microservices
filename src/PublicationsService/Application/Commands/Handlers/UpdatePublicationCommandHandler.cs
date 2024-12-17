using MediatR;
using PublicationsService.Application.DTO;
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
    public class UpdatePublicationCommandHandler : IRequestHandler<UpdatePublicationCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly IPublicationDomain _publicationDomain;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        private readonly IHttpContextAccessor _contextAccessor;
        #endregion

        #region Constructor
        public UpdatePublicationCommandHandler(
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
        public async Task<IEndpointResponse<IDatabaseResult>> Handle(UpdatePublicationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var publication = new PublicationUpdateDTO
                {
                    IdPublication = request.IdPubliaction,
                    IdUser = request.IdUser,
                    IdRole = request.IdRole,
                    Title = request.Title,
                    Description = request.Description,
                    Status = request.Status,
                    Salary = request.Salary,
                    Location = request.Location,
                    Company = request.Company,
                };

                var response = await _publicationDomain.UpdatePublicationAsync(publication);
                _endpointResponse.Result = response;

                if (response == null || !response.ResultStatus)
                {
                    throw new Exception($"Publication not found {request.IdPubliaction}");
                }

                _endpointResponse.IsSuccess = true;
                _endpointResponse.Message = "Publication updated successful";

                await _eventPublisherService.PublishEventAsync(
                    entityName: AuditEntityType.Publication.ToEntityName(),
                    operationType: AuditOperationType.Update.ToOperationType(),
                    success: true,
                    performedBy: _contextAccessor.GtePerformedBy(),
                    reason: response?.ResultMessage ?? "Publication not found",
                    additionalData: publication,
                    exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Update_Success.ToRoutingKey()
                    );
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Handler, ActionType.Update);
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = $"Error updating publication: {ex.Message}";

                var eventError = new RegisterErrorEvent
                {
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };

                await _eventPublisherService.PublishEventAsync(
                    entityName: AuditEntityType.Publication.ToEntityName(),
                    operationType: AuditOperationType.Update.ToOperationType(),
                    success: false,
                    performedBy: _contextAccessor.GtePerformedBy(),
                    reason: ex.Message,
                    additionalData: eventError,
                    exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Update_Error.ToRoutingKey()
                    );
            }
            return _endpointResponse;
        }
        #endregion
    }
}
