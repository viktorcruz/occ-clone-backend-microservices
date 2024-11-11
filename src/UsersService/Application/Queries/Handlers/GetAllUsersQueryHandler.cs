using MediatR;
using SharedKernel.Common.Extensions;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using UsersService.Application.Dto;
using UsersService.Domain.Interface;

namespace UsersService.Application.Queries.Handlers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEndpointResponse<RetrieveDatabaseResult<List<UserRetrieveDTO>>>>
    {
        #region Properties
        private readonly IUserDomain _usersDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<RetrieveDatabaseResult<List<UserRetrieveDTO>>> _endpointResponse;
        private readonly IEventPublisherService _eventPublisherService;
        #endregion

        #region Constructor
        public GetAllUsersQueryHandler(
            IUserDomain usersDomain,
            IGlobalExceptionHandler globalExceptionHandler,
            IEndpointResponse<RetrieveDatabaseResult<List<UserRetrieveDTO>>> endpointResponse,
            IEventPublisherService eventPublisherService
            )
        {
            _usersDomain = usersDomain;
            _globalExceptionHandler = globalExceptionHandler;
            _endpointResponse = endpointResponse;
            _eventPublisherService = eventPublisherService;
        }
        #endregion

        #region Methods
        public async Task<IEndpointResponse<RetrieveDatabaseResult<List<UserRetrieveDTO>>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _usersDomain.GetAllUsersAsync();
                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "Successful";

                    var additionalData = new
                    {
                        TotalUsers = response.Details.Count,
                    };

                    await _eventPublisherService.PublishEventAsync(
                        entityName: "User",
                        operationType: "GETALL",
                        success: true,
                        performedBy: "Admin",
                        reason: response?.ResultMessage,
                        additionalData: additionalData,
                        exchangeName: PublicationExchangeNames.Users.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.GetAll_Success.ToRoutingKey()
                        );
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = "Users not found";

                    await _eventPublisherService.PublishEventAsync(
                        entityName: "User",
                        operationType: "GETALL",
                        success: false,
                        performedBy: "Admin",
                        reason: response?.ResultMessage ?? "User not found",
                        additionalData: response,
                        exchangeName: PublicationExchangeNames.Users.ToExchangeName(),
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
                    entityName: "User",
                    operationType: "GETALL",
                    success: false,
                    performedBy: "Admin",
                    reason: "Data not found",
                    exchangeName: PublicationExchangeNames.Users.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.GetAll_Error.ToRoutingKey()
                    );
            }
            return _endpointResponse;
        }
        #endregion
    }
}
