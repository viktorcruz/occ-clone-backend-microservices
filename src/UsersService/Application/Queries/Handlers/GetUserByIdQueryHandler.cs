using AutoMapper;
using MediatR;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using UsersService.Application.Dto;
using UsersService.Domain.Events;
using UsersService.Domain.Interface;
using UsersService.Infrastructure.Messaging;

namespace UsersService.Application.Queries.Handlers
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>>>
    {
        #region Properties
        private readonly IUserDomain _userDomain;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>> _endpointResponse;
        private readonly EventBusRabbitMQ _eventBus;
        #endregion

        #region Constructor
        public GetUserByIdQueryHandler(
            IUserDomain userDomain,
            IMapper mapper,
            IGlobalExceptionHandler globalExceptionHandler,
            IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>> endpointResponse,
            EventBusRabbitMQ eventBus
            )
        {
            _userDomain = userDomain;
            _globalExceptionHandler = globalExceptionHandler;
            _endpointResponse = endpointResponse;
            _eventBus = eventBus;
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

                    var entityOperationEvent = new EntityOperationEvent(
                        entityName: "User",
                        operationType: "Get By Id",
                        success: true,
                        performedBy: "Admin",
                        reason: response.ResultStatus.ToString(),
                        additionalData: additionalData
                        );

                    _eventBus.Publish("UserExchange", "UserGetByIdEvent", entityOperationEvent);

                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = "User not found";
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "GetUserByIdQuery.Handle");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = ex.Message;

                var failedEvent = new EntityOperationEvent(entityName: "User", operationType: "GetById", success: false, performedBy: "AdminUser");

                _eventBus.Publish("UserExchange", "UserGetByIdFailed", failedEvent);

            }
            return _endpointResponse;
        }
        #endregion
    }
}
