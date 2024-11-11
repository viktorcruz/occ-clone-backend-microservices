using AutoMapper;
using MediatR;
using SharedKernel.Common.Extensions;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using UsersService.Application.Dto;
using UsersService.Domain.Interface;

namespace UsersService.Application.Queries.Handlers
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>>>
    {
        #region Properties
        private readonly IUserDomain _userDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>> _endpointResponse;
        private readonly IEventPublisherService _eventPublisherService;
        #endregion

        #region Constructor
        public GetUserByIdQueryHandler(
            IUserDomain userDomain,
            IMapper mapper,
            IGlobalExceptionHandler globalExceptionHandler,
            IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>> endpointResponse,
            IEventPublisherService eventPublisherService
            )
        {
            _userDomain = userDomain;
            _globalExceptionHandler = globalExceptionHandler;
            _endpointResponse = endpointResponse;
            _eventPublisherService = eventPublisherService;
        }
        #endregion

        #region Methods
        public async Task<IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _userDomain.GetUserByIdAsync(request.IdUser);
                _endpointResponse.Result = response;

                if (response != null && response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "Successful";

                    var additionalData = new
                    {
                        IdUser = request.IdUser,
                        FirstName = response.Details.FirstName,
                        LastName = response.Details.LastName,
                        Email = response.Details.Email,
                        CreatedAt = DateTime.Now
                    };

                    await _eventPublisherService.PublishEventAsync(
                        entityName: "User",
                        operationType: "GET",
                        success: true,
                        performedBy: "Admin",
                        reason: response?.ResultMessage,
                        additionalData: additionalData,
                        exchangeName: PublicationExchangeNames.Users.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.Get_Success.ToRoutingKey()
                        );
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = "User not found";

                    await _eventPublisherService.PublishEventAsync(
                        entityName: "User",
                        operationType: "GET",
                        success: false,
                        performedBy: "Admin",
                        reason: response?.ResultMessage ?? "User nor found",
                        additionalData: response,
                        exchangeName: PublicationExchangeNames.Users.ToExchangeName(),
                        routingKey: PublicationRoutingKeys.GetAll_Failed.ToRoutingKey()
                        );
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "GetUserByIdQuery.Handle");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = ex.Message;

                await _eventPublisherService.PublishEventAsync(
                    entityName: "User",
                    operationType: "GET",
                    success: false,
                    performedBy: "Admin",
                    reason: "User nor found",
                    additionalData: null,
                    exchangeName: PublicationExchangeNames.Users.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Get_Error.ToRoutingKey()
                    );
            }
            return _endpointResponse;
        }
        #endregion
    }
}
