using MediatR;
using SharedKernel.Common.Response;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using UsersService.Domain.Interface;

namespace UsersService.Application.Commands
{
    public class DeleteUserCommand
    {
        public class TaskCommand : IRequest<IEndpointResponse<DatabaseResult>>
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

        public class DeleteUserCommandHandler : IRequestHandler<TaskCommand, IEndpointResponse<DatabaseResult>>
        {
            #region Properties
            private readonly IUserDomain _usersDomain;
            private readonly IGlobalExceptionHandler _globalExceptionHandler;
            #endregion

            #region Constructor
            public DeleteUserCommandHandler(IUserDomain usersDomain, IGlobalExceptionHandler globalExceptionHandler)
            {
                _usersDomain = usersDomain;
                _globalExceptionHandler = globalExceptionHandler;
            }
            #endregion

            #region Methods
            public async Task<IEndpointResponse<DatabaseResult>> Handle(TaskCommand request, CancellationToken cancellationToken)
            {
                var endpointResponse = new EndpointResponse<DatabaseResult>();

                try
                {
                    var response = await _usersDomain.DeleteUserAsync(request.idUser);
                    endpointResponse.Data = response;

                    if (response != null && response.ResultStatus)
                    {
                        endpointResponse.IsSuccess = true;
                        endpointResponse.Message = "User deleted successful";
                    }
                    else
                    {
                        endpointResponse.IsSuccess = false;
                        endpointResponse.Message = response?.ResultMessage ?? "User not found";
                    }
                }
                catch (Exception ex)
                {
                    _globalExceptionHandler.HandleGenericException<string>(ex, "DeleteUserCommand.Handle");
                    endpointResponse.IsSuccess = false;
                    endpointResponse.Message = $"Error deleting user: {ex.Message}";
                }
                return endpointResponse;
            }
            #endregion
        }
    }
}
