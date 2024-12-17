using SearchJobsService.Application.Commands;
using SearchJobsService.Application.Commands.Handler.Apply;
using SearchJobsService.Domain.Interface;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Common.Messaging;
using SharedKernel.Events.JobSearch;
using SharedKernel.Extensions.Routing;
using SharedKernel.Interfaces.Exceptions;

namespace SearchJobsService.Saga
{
    public class SagaOrchestrator
    {
        #region Properties
        private readonly ISearchJobsDomain _searchJobsDomain;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        private readonly IEventPublisherService _eventPublisherService;
        private readonly ILogger<RabbitMQEventBus> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        #endregion

        public SagaOrchestrator(
            ISearchJobsDomain searchJobsDomain,
            IApplicationExceptionHandler applicationExceptionHandler,
            IEventPublisherService eventPublisherService,
            ILogger<RabbitMQEventBus> logger,
            IHttpContextAccessor contextAccessor
        )
        {
            _searchJobsDomain = searchJobsDomain;
            _applicationExceptionHandler = applicationExceptionHandler;
            _eventPublisherService = eventPublisherService;
            _logger = logger;
            _contextAccessor = contextAccessor;
        }

        public async Task StartSaga(int jobId, int userId)
        {
            try
            {
                Console.WriteLine("[Saga] Starting Saga for job application");

                var applyJobCommand = new ApplyCommand
                {
                    IdPublication = jobId,
                    IdApplicant = userId,
                    ApplicantName = "",
                    ApplicantResume = "",
                    CoverLetter = "",
                    ApplicationDate = DateTime.Now, 
                    Status = 0,
                    StatusMessage = "OK",
                };

                // Ejecutar flujo de postulación
                var handler = new ApplyCommandHandler(
                    _searchJobsDomain,
                    _applicationExceptionHandler,
                    _logger,
                    _eventPublisherService,
                    _contextAccessor
                    );
                var cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = cancellationTokenSource.Token;

                await handler.Handle(applyJobCommand, cancellationToken);

                Console.WriteLine("[Saga] Saga completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Saga] Saga failed: {ex.Message}");

                var failedEvent = new JobApplicationFailedEvent
                {
                    IdJob = jobId,
                    IdApplicant = userId,
                    Reason = ex.Message,
                    FailedAt = DateTime.UtcNow
                };

                await _eventPublisherService.PublishEventAsync(
                        entityName: "Job",
                        operationType: "APPLY",
                        success: false,
                        performedBy: "Admin",
                        reason: ex.Message,
                        additionalData: failedEvent,
                        exchangeName: PublicationExchangeNames.Job.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Apply_Error.ToRoutingKey()
                    );
            }
        }
    }
}
