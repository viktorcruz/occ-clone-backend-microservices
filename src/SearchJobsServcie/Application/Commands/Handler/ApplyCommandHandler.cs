using MediatR;
using SearchJobsService.Domain.Enum;
using SearchJobsService.Domain.Interface;
using SharedKernel.Common.Extensions;
using SharedKernel.Interface;

namespace SearchJobsService.Application.Commands.Handler
{
    public class ApplyCommandHandler : IRequestHandler<ApplyCommand, IEndpointResponse<IDatabaseResult>>
    {
        #region Properties
        private readonly ISearchJobsDomain _searchJobsDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
        private readonly IEventPublisherService _eventPublisherService;
        #endregion

        #region Constructor
        public ApplyCommandHandler(
            ISearchJobsDomain searchJobsDomain,
            IGlobalExceptionHandler globalExceptionHandler,
            IEndpointResponse<IDatabaseResult> endpointResponse,
            IEventPublisherService eventPublisherService
            )
        {
            _searchJobsDomain = searchJobsDomain;
            _globalExceptionHandler = globalExceptionHandler;
            _endpointResponse = endpointResponse;
            _eventPublisherService = eventPublisherService;
        }
        #endregion

        #region Methods
        public async Task<IEndpointResponse<IDatabaseResult>> Handle(ApplyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _searchJobsDomain.ApplyAsync(request);

                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "Application successful";

                    var additionalData = new
                    {
                        IdPublication = request.IdPublication,
                        IdApplicant = request.IdApplicant,
                        ApplicantName = request.ApplicantName,
                        ApplicantResume = request.ApplicantResume,
                        CoverLetter = request.CoverLetter,
                        ApplicationDate = request.ApplicationDate,
                        Status = RecruitmentStatus.Applied.ToString().ToLower(),
                    };

                    await _eventPublisherService.PublishEventAsync(
                        entityName: "SearchJobs",
                        operationType: "APPLY",
                        success: true,
                        performedBy: "Admin",
                        reason: response?.ResultMessage ?? "Application not found",
                        additionalData: additionalData,
                        exchangeName: PublicationExchangeNames.SearchJobs.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Apply_Success.ToRoutingKey()
                        );
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = response?.ResultMessage ?? "Apply not created";

                    await _eventPublisherService.PublishEventAsync(
                        entityName: "Search Jobs",
                        operationType: "SEARCH",
                        success: false,
                        performedBy: "Admin",
                        reason: "Apply not found",
                        additionalData: null,
                        exchangeName: PublicationExchangeNames.SearchJobs.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Apply_Failed.ToRoutingKey()
                        );
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "ApplyCommandHandler");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = $"Error apply: {ex.Message}";

                await _eventPublisherService.PublishEventAsync(
                        entityName: "SearchJobs",
                        operationType: "APPLY",
                        success: false,
                        performedBy: "Admin",
                        reason: ex.Message,
                        additionalData: null,
                        exchangeName: PublicationExchangeNames.SearchJobs.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Apply_Error.ToRoutingKey()
                    );
            }
            return _endpointResponse;
        }
        #endregion
    }
}
