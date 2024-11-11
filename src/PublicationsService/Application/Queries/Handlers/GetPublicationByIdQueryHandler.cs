using MediatR;
using PublicationsService.Aplication.Dto;
using PublicationsService.Domain.Interface;
using SharedKernel.Common.Extensions;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

namespace PublicationsService.Aplication.Queries.Handlers
{
    public class GetPublicationByIdQueryHandler : IRequestHandler<GetPublicationByIdQuery, IEndpointResponse<RetrieveDatabaseResult<PublicationRetrieveDTO>>>
    {
        #region Properties
        private readonly IPublicationDomain _publicationDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<RetrieveDatabaseResult<PublicationRetrieveDTO>> _endpointResponse;
        private readonly IEventPublisherService _eventPublisherService;
        #endregion

        #region Constructor
        public GetPublicationByIdQueryHandler(
            IPublicationDomain publicationDomain,
            IGlobalExceptionHandler globalExceptionHandler,
            IEndpointResponse<RetrieveDatabaseResult<PublicationRetrieveDTO>> endpointResponse,
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
        public async Task<IEndpointResponse<RetrieveDatabaseResult<PublicationRetrieveDTO>>> Handle(GetPublicationByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _publicationDomain.GetPublicationByIdAsync(request.IdPublication);
                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "Successful";

                    var additionalData = new
                    {
                        IdPublication = request.IdPublication,
                        IdUser = response.Details.IdUser,
                        IdRole = response.Details.IdRole,
                        Title = response.Details.Title,
                        Description = response.Details.Description,
                        PublicationDate = response.Details.PublicationDate,
                        ExpirationDate = response.Details.ExpirationDate,
                        Status = response.Details.Status,
                        Location = response.Details.Location,
                        Company = response.Details.Company,
                    };

                    await _eventPublisherService.PublishEventAsync(
                        entityName: "Publication",
                        operationType: "GET",
                        success: true,
                        performedBy: "Admin",
                        reason: response.ResultMessage,
                        additionalData: additionalData,
                        exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.GetAll_Success.ToRoutingKey()
                        );
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = response?.ResultMessage ?? "Publication not found";

                    await _eventPublisherService.PublishEventAsync(
                        entityName: "Publication",
                        operationType: "GET",
                        success: false,
                        performedBy: "Admin",
                        additionalData: null,
                        exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Get_Failed.ToRoutingKey()
                        );
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "GetByIdPublicationQueryHandler");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = $"Error getting publication: {ex.Message}";

                await _eventPublisherService.PublishEventAsync(
                        entityName: "Publication",
                        operationType: "GET",
                        success: false,
                        performedBy: "Admin",
                        reason: ex.Message,
                        additionalData: null,
                        exchangeName: PublicationExchangeNames.Publication.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Get_Error.ToRoutingKey()
                    );
            }
            return _endpointResponse;
        }
        #endregion
    }
}
