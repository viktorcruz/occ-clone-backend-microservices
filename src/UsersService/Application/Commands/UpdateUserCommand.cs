using MediatR;
using UsersService.Application.Dto;
using UsersService.Domain.Interface;
using UsersService.SharedKernel.Common.Response;
using UsersService.SharedKernel.Interface;

namespace UsersService.Application.Commands
{
    public class UpdateUserCommand
    {
        public class TaskCommand: IRequest<IApiResponse<SpResult>>
        {
            #region Properties
            public int IdUser { get; set; }
            public int IdRole { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            #endregion

            #region Constructor
            public TaskCommand(UserRetrieveDTO userDTO)
            {
                IdUser = userDTO.IdUser;
                IdRole = userDTO.IdRole;
                FirstName = userDTO.FirstName;
                LastName = userDTO.LastName;    
                Email = userDTO.Email;  
            }
            #endregion
        }

        public class UpdateUserCommandHandler : IRequestHandler<TaskCommand, IApiResponse<SpResult>>
        {
            #region Properties
            private readonly IUsersDomain _usersDomain;
            private readonly IExceptionManagement _exceptionManagement;
            #endregion

            #region Constructor
            public UpdateUserCommandHandler(IUsersDomain usersDomain, IExceptionManagement exceptionManagement)
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
                    var user = new UserRetrieveDTO
                    {
                        IdUser = request.IdUser,
                        IdRole = request.IdRole,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Email = request.Email,
                    };

                    var response = await _usersDomain.UpdateUserAsync(user);
                    apiResponse.Data = response;

                    if(response != null && response.ResultStatus)
                    {
                        apiResponse.IsSuccess = true;
                        apiResponse.Message = "User updated sucessful";
                    }
                    else
                    {
                        apiResponse.IsSuccess = false;
                        apiResponse.Message = response?.ResultMessage ?? "User not found";
                    }
                }
                catch(Exception ex)
                {
                    _exceptionManagement.HandleGenericException<string>(ex, "UpdateUserCommand.Handle");
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = $"Error updateing user: {ex.Message}";
                }
                return apiResponse;
            }
            #endregion
        }
    }
}
