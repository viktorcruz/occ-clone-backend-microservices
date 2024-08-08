using MediatR;
using SharedKernel.Common.Response;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using UsersService.Application.Dto;
using UsersService.Domain.Interface;

namespace UsersService.Application.Commands
{
    public class CreateUserCommand
    {
        public class TaskCommand : IRequest<IEndpointResponse<DatabaseResult>>
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

        public class CreateUserCommandHandler : IRequestHandler<TaskCommand, IEndpointResponse<DatabaseResult>>
        {
            #region Properties
            private readonly IUserDomain _userDomain;
            private readonly IGlobalExceptionHandler _globalExceptionHandler;
            #endregion

            #region Constructor
            public CreateUserCommandHandler(IUserDomain usersDomain, IGlobalExceptionHandler globalExceptionHandler)
            {
                _userDomain = usersDomain;
                _globalExceptionHandler = globalExceptionHandler;
            }
            #endregion

            #region Methods
            public async Task<IEndpointResponse<DatabaseResult>> Handle(TaskCommand request, CancellationToken cancellationToken)
            {
                var endpointResponse = new EndpointResponse<DatabaseResult>();

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
                    endpointResponse.Data = response;

                    if (response != null && response.ResultStatus)
                    {
                        endpointResponse.IsSuccess = true;
                        endpointResponse.Message = "User created successful";
                    }
                    else
                    {
                        endpointResponse.IsSuccess = false;
                        endpointResponse.Message = response?.ResultMessage ?? "User not found";
                    }
                }
                catch (Exception ex)
                {
                    _globalExceptionHandler.HandleGenericException<string>(ex, "CreateUserCommand.Handle");
                    endpointResponse.IsSuccess = false;
                    endpointResponse.Message = $"Error creating user: {ex.Message}";
                }
                return endpointResponse;
            }
            #endregion
        }

    }
}
