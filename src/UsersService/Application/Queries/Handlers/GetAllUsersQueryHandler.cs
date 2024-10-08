﻿using MediatR;
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
        #endregion

        #region Constructor
        public GetAllUsersQueryHandler(IUserDomain usersDomain, IGlobalExceptionHandler globalExceptionHandler, IEndpointResponse<RetrieveDatabaseResult<List<UserRetrieveDTO>>> endpointResponse)
        {
            _usersDomain = usersDomain;
            _globalExceptionHandler = globalExceptionHandler;
            _endpointResponse = endpointResponse;
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
                }
                else
                {
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = "Users not found";
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "GetAllUserQuery.Handle");
                _endpointResponse.IsSuccess = false;
                _endpointResponse.Message = ex.Message;
            }
            return _endpointResponse;
        }
        #endregion
    }
}
