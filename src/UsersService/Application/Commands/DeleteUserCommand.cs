using MediatR;
using SharedKernel.Common.Response;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using UsersService.Domain.Interface;

namespace UsersService.Application.Commands
{
    public class DeleteUserCommand
    {
        public class TaskCommand : IRequest<IEndpointResponse<IDatabaseResult>>
        {
            #region Properties
            public int idUser { get; set; }
            #endregion

            #region Constructor
            public TaskCommand(int userId)
            {
                idUser = userId;
            }
            #endregion
        }

        public class DeleteUserCommandHandler : IRequestHandler<TaskCommand, IEndpointResponse<IDatabaseResult>>
        {
            #region Properties
            private readonly IUserDomain _usersDomain;
            private readonly IGlobalExceptionHandler _globalExceptionHandler;
            private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
            #endregion

            #region Constructor
            public DeleteUserCommandHandler(IUserDomain usersDomain, IGlobalExceptionHandler globalExceptionHandler, IEndpointResponse<IDatabaseResult> endpointResponse)
            {
                _usersDomain = usersDomain;
                _globalExceptionHandler = globalExceptionHandler;
                _endpointResponse = endpointResponse;

            }
            #endregion

            #region Methods
            public async Task<IEndpointResponse<IDatabaseResult>> Handle(TaskCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var response = await _usersDomain.DeleteUserAsync(request.idUser);
                    _endpointResponse.Result = response;

                    if (response != null && response.ResultStatus)
                    {
                        _endpointResponse.IsSuccess = true;
                        _endpointResponse.Message = "User deleted successful";
                    }
                    else
                    {
                        _endpointResponse.IsSuccess = false;
                        _endpointResponse.Message = response?.ResultMessage ?? "User not found";
                    }
                }
                catch (Exception ex)
                {
                    _globalExceptionHandler.HandleGenericException<string>(ex, "DeleteUserCommand.Handle");
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = $"Error deleting user: {ex.Message}";
                }
                return _endpointResponse;
            }
            #endregion
        }
    }
}
