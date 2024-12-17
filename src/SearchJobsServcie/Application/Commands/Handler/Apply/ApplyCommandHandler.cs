using MediatR;
using SearchJobsService.Domain.Interface;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Common.Messaging;
using SharedKernel.Common.Response;
using SharedKernel.Events.JobSearch;
using SharedKernel.Extensions.Event;
using SharedKernel.Extensions.Http;
using SharedKernel.Extensions.Routing;
using SharedKernel.Interfaces.Exceptions;
using SharedKernel.Interfaces.Response;

namespace SearchJobsService.Application.Commands.Handler.Apply
{
    public class ApplyCommandHandler : IRequestHandler<ApplyCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly ISearchJobsDomain _searchJobsDomain;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        private readonly ILogger<RabbitMQEventBus> _logger;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly IHttpContextAccessor _contextAccessor;
        #endregion

        #region Constructor
        public ApplyCommandHandler(
            ISearchJobsDomain searchJobsDomain,
            IApplicationExceptionHandler applicationExceptionHandler,
            ILogger<RabbitMQEventBus> logger,
            IEventPublisherService eventPublisherService,
            IHttpContextAccessor contextAccessor
            )
        {
            _searchJobsDomain = searchJobsDomain;
            _applicationExceptionHandler = applicationExceptionHandler;
            _logger = logger;
            _eventPublisherService = eventPublisherService;
            _contextAccessor = contextAccessor;
        }
        #endregion

        #region Methods
        public async Task<IEndpointResponse<IDatabaseResult>> Handle(ApplyCommand request, CancellationToken cancellationToken)
        {
            var endpointResponse = new EndpointResponse<IDatabaseResult>();

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                Console.WriteLine($"[Handler] Registering job application: UserId={request.IdApplicant}, JobId={request.IdApplicant}");
                _logger.LogInformation($"[Handler] Registering job application: UserId={request.IdApplicant}, JobId={request.IdApplicant}");

                var alreadyApplied = await _searchJobsDomain.HasApplicationAsync(request.IdApplicant, request.IdPublication);

                if (alreadyApplied == null)
                {
                    throw new Exception($"[Handler] Applicant {request.IdApplicant} has already applied for job");
                }

                var response = await _searchJobsDomain.ApplyAsync(request);

                Console.WriteLine("[Handler] Job application registered successfully");
                _logger.LogInformation($"[Handler] Registering job application: UserId={request.IdApplicant}, JobId={request.IdApplicant}");

                if (response != null && response.ResultStatus)
                {
                    Console.WriteLine("[Handler] Job application registered successfully.");
                    _logger.LogInformation($"[Handler] Job application registered successfully.");

                    endpointResponse.IsSuccess = true;
                    endpointResponse.Message = "Application successful";
                    endpointResponse.Result = response;

                    var jobAppliedEvent = new JobAppliedEvent
                    {
                        IdJob = response.AffectedRecordId,
                        IdUser = request.IdApplicant,
                        AppliedAt = DateTime.UtcNow
                    };

                    await _eventPublisherService.PublishEventAsync(
                        entityName: AuditEntityType.Job.ToEntityName(),
                        operationType: AuditOperationType.Apply.ToOperationType(),
                        success: true,
                        performedBy: _contextAccessor.GtePerformedBy(),
                        reason: response?.ResultMessage ?? "Application not found",
                        additionalData: jobAppliedEvent,
                        exchangeName: PublicationExchangeNames.Job.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Apply_Success.ToRoutingKey()
                        );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Handler] Error during job application: {ex.Message}");
                _logger.LogInformation($"[Handler] Error during job application: {ex.Message}");

                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Handler, ActionType.Apply);

                endpointResponse.IsSuccess = false;
                endpointResponse.Message = $"Error apply: {ex.Message}";

                var failedEvent = new JobApplicationFailedEvent
                {
                    IdJob = request.IdPublication,
                    IdApplicant = request.IdApplicant,
                    Reason = ex.Message,
                    FailedAt = DateTime.UtcNow
                };
                await _eventPublisherService.PublishEventAsync(
                        entityName: AuditEntityType.Job.ToEntityName(),
                        operationType: AuditOperationType.Apply.ToOperationType(),
                        success: false,
                        performedBy: _contextAccessor.GtePerformedBy(),
                        reason: ex.Message,
                        additionalData: failedEvent,
                        exchangeName: PublicationExchangeNames.Job.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Apply_Error.ToRoutingKey()
                    );
            }
            return endpointResponse;
        }
        #endregion
    }
}
