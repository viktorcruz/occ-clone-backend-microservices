using MediatR;
using UsersService.Domain.Interface;
using UsersService.SharedKernel.Common.Response;
using UsersService.SharedKernel.Interface;

namespace UsersService.Application.Commands
{
    public class DeleteUserCommand
    {
        public class TaskCommand : IRequest<IApiResponse<SpResult>>
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

        public class DeleteUserCommandHandler : IRequestHandler<TaskCommand, IApiResponse<SpResult>>
        {
            #region Properties
            private readonly IUsersDomain _usersDomain;
            private readonly IExceptionManagement _exceptionManagement;
            #endregion

            #region Constructor
            public DeleteUserCommandHandler(IUsersDomain usersDomain, IExceptionManagement exceptionManagement)
            {
                _usersDomain = usersDomain;
                _exceptionManagement = exceptionManagement;
            }
            #endregion

            #region Methods
            public async Task<IApiResponse<SpResult>> Handle(TaskCommand request, CancellationToken cancellationToken)
            {
                var apiResponse = new ApiResponse<SpResult>();

                try
                {
                    var response = await _usersDomain.DeleteUserAsync(request.idUser);
                    apiResponse.Data = response;

                    if (response != null && response.ResultStatus)
                    {
                        apiResponse.IsSuccess = true;
                        apiResponse.Message = "User deleted successful";
                    }
                    else
                    {
                        apiResponse.IsSuccess = false;
                        apiResponse.Message = response?.ResultMessage ?? "User not found";
                    }
                }
                catch (Exception ex)
                {
                    _exceptionManagement.HandleGenericException<string>(ex, "DeleteUserCommand.Handle");
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = $"Error deleting user: {ex.Message}";
                }
                return apiResponse;
            }
            #endregion
        }
    }
}
