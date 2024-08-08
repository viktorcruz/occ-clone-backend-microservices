using MediatR;
using UsersService.Application.Dto;
using UsersService.Domain.Interface;
using UsersService.SharedKernel.Common.Response;
using UsersService.SharedKernel.Interface;

namespace UsersService.Application.Commands
{
    public class CreateUserCommand
    {
        public class TaskCommand : IRequest<IApiResponse<SpResult>>
        {
            #region Properties
            public int IdRole { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            #endregion

            #region Constructor
            public TaskCommand(AddUserDTO userDTO)
            {
                IdRole = userDTO.IdRole;
                FirstName = userDTO.FirstName;
                LastName = userDTO.LastName;
                Email = userDTO.Email;
            }
            #endregion
        }

        public class CreateUserCommandHandler : IRequestHandler<TaskCommand, IApiResponse<SpResult>>
        {
            #region Properties
            private readonly IUsersDomain _userDomain;
            private readonly IExceptionManagement _exceptionManagement;
            #endregion

            #region Constructor
            public CreateUserCommandHandler(IUsersDomain usersDomain, IExceptionManagement exceptionManagement)
            {
                _userDomain = usersDomain;
                _exceptionManagement = exceptionManagement;
            }
            #endregion

            #region Methods
            public async Task<IApiResponse<SpResult>> Handle(TaskCommand request, CancellationToken cancellationToken)
            {
                var apiResponse = new ApiResponse<SpResult>();

                try
                {
                    var user = new AddUserDTO
                    {
                        IdRole = request.IdRole,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Email = request.Email,  
                    };

                    var response = await _userDomain.CreateUserAsync(user);
                    apiResponse.Data = response;

                    if (response != null && response.ResultStatus)
                    {
                        apiResponse.IsSuccess = true;
                        apiResponse.Message = "User created successful";
                    }
                    else
                    {
                        apiResponse.IsSuccess = false;
                        apiResponse.Message = response?.ResultMessage ?? "User not found";
                    }
                }
                catch (Exception ex)
                {
                    _exceptionManagement.HandleGenericException<string>(ex, "CreateUserCommand.Handle");
                    apiResponse.IsSuccess = false;
                    apiResponse.Message = $"Error creating user: {ex.Message}";
                }
                return apiResponse;
            }
            #endregion
        }

    }
}
