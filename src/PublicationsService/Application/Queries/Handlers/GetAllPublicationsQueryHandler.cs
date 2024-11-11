using MediatR;
using PublicationsService.Aplication.Dto;
using PublicationsService.Domain.Events;
using PublicationsService.Domain.Interface;
using SharedKernel.Common.Extensions;
using SharedKernel.Common.Interfaces;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

namespace PublicationsService.Aplication.Queries.Handlers
{
    public class GetAllPublicationsQueryHandler : IRequestHandler<GetAllPulicationsQuery, IEndpointResponse<RetrieveDatabaseResult<List<PublicationRetrieveDTO>>>>
    {
        #region Properties
        private readonly IPublicationDomain _publicationDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<RetrieveDatabaseResult<List<PublicationRetrieveDTO>>> _endpointResponse;
        private readonly IEventPublisherService _eventPublisherService;
        #endregion

        #region Constructor
        public GetAllPublicationsQueryHandler(
            IPublicationDomain publicationDomain,
            IGlobalExceptionHandler globalExceptionHandler,
            IEndpointResponse<RetrieveDatabaseResult<List<PublicationRetrieveDTO>>> endpointResponse,
            IEventPublisherService eventPublisherService
            )
        {
            _publicationDomain = publicationDomain;
            _globalExceptionHandler = globalExceptionHandler;
            _endpointResponse = endpointResponse;
            _eventPublisherService = eventPublisherService;
        }
        #endregion

        #region Methods
        public async Task<IEndpointResponse<RetrieveDatabaseResult<List<PublicationRetrieveDTO>>>> Handle(GetAllPulicationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _publicationDomain.GetAllPublicationAsync();
                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "Successful";

                    var additionalData = new
                    {
                        TotalPublications = response.Details.Count,
                    };

                    await _eventPublisherService.PublishEventAsync(
                        entityName: "Publication",
                        operationType: "GETALL",
                        success: true,
                        performedBy: "Admin",
                        reason: response?.ResultMessage,
                        additionalData: additionalData,
                        exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.GetAll_Success.ToRoutingKey()
                        );
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = "Publications not found";

                    await _eventPublisherService.PublishEventAsync(
                        entityName: "Publication",
                        operationType: "GETALL",
                        success: false,
                        performedBy: "Admin",
                        reason: response?.ResultMessage ?? "Publications not found",
                        additionalData: null,
                        exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.GetAll_Failed.ToRoutingKey()
                    );
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "GetAllUserQuery.Handle");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = ex.Message;

                await _eventPublisherService.PublishEventAsync(
                    entityName: "Publication",
                    operationType: "GETALL",
                    success: false,
                    performedBy: "Admin",
                    reason: ex.Message,
                    additionalData: null,
                    exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.GetAll_Error.ToRoutingKey()
                    );
            }
            return _endpointResponse;
        }
        #endregion
    }
}
