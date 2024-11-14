using MediatR;
using PublicationsService.Domain.Interface;
using SharedKernel.Common.Events;
using SharedKernel.Common.Extensions;
using SharedKernel.Common.Interfaces;
using SharedKernel.Interface;

namespace PublicationsService.Aplication.Commands.Handlers
{
    public class CreatePublicationCommandHandler : IRequestHandler<CreatePublicationCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly IPublicationDomain _publicationDomain;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        #endregion

        #region Constructor
        public CreatePublicationCommandHandler(
            IPublicationDomain publicationDomain,
            IEventPublisherService eventPublisherService,
            IApplicationExceptionHandler applicationExceptionHandler,
            IEndpointResponse<IDatabaseResult> endpointResponse
            )
        {
            _publicationDomain = publicationDomain;
            _eventPublisherService = eventPublisherService;
            _applicationExceptionHandler = applicationExceptionHandler;
            _endpointResponse = endpointResponse;
        }
        #endregion

        #region Methods
        public async Task<IEndpointResponse<IDatabaseResult>> Handle(CreatePublicationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _publicationDomain.CreatePublicationAsync(request);

                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
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
                        entityName: "Publication",
                        operationType: "CREATE",
                        success: true,
                        performedBy: "Admin",
                        reason: response?.ResultMessage,
                        additionalData: additionalData,
                        "publication_exchange",
                        "publication.created"
                        );
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = response?.ResultMessage ?? "Publication not created";

                    await _eventPublisherService.PublishEventAsync(
                        entityName: "Publication",
                        operationType: "CREATE",
                        success: false,
                        performedBy: "Admin",
                        reason: response?.ResultMessage,
                        additionalData: null,
                        exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Create_Failed.ToRoutingKey()
                        );
                }
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
                    entityName: "Publication",
                    operationType: "CREATE",
                    success: false,
                    performedBy: "Admin",
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
