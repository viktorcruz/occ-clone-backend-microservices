using AutoMapper;
using MediatR;
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
        #endregion

        #region Constructor
        public GetUserByIdQueryHandler(IUserDomain userDomain, IMapper mapper, IGlobalExceptionHandler globalExceptionHandler, IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>> endpointResponse)
        {
            _userDomain = userDomain;
            _globalExceptionHandler = globalExceptionHandler;
            _endpointResponse = endpointResponse;
        }
        #endregion


        #region Methods
        public async Task<IEndpointResponse<RetrieveDatabaseResult<UserRetrieveDTO>>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _userDomain.GetUserByIdAsync(request.IdUser);
                _endpointResponse.Result = response;

                //_endpointResponse.IsSuccess = response != null && response.ResultStatus;
                //_endpointResponse.Message = _endpointResponse.IsSuccess ? "Successful" : "User not found";

                if (response != null && response.ResultStatus)
                {
                    _endpointResponse.IsSuccess = true;
                    _endpointResponse.Message = "Successful";
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
            }
            return _endpointResponse;
        }
        #endregion
    }
}
