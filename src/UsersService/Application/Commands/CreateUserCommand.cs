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
        public class TaskCommand : IRequest<IEndpointResponse<IDatabaseResult>>
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

        public class CreateUserCommandHandler : IRequestHandler<TaskCommand, IEndpointResponse<IDatabaseResult>>
        {
            #region Properties
            private readonly IUserDomain _userDomain;
            private readonly IGlobalExceptionHandler _globalExceptionHandler;
            private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
            #endregion

            #region Constructor
            public CreateUserCommandHandler(IUserDomain usersDomain, IGlobalExceptionHandler globalExceptionHandler, IEndpointResponse<IDatabaseResult> endpointResponse)
            {
                _userDomain = usersDomain;
                _globalExceptionHandler = globalExceptionHandler;
                _endpointResponse = endpointResponse;

            }
            #endregion

            #region Methods
            public async Task<IEndpointResponse<IDatabaseResult>> Handle(TaskCommand request, CancellationToken cancellationToken)
            {
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
                    _endpointResponse.Result = response;

                    if (response != null && response.ResultStatus)
                    {
                        _endpointResponse.IsSuccess = true;
                        _endpointResponse.Message = "User created successful";
                    }
                    else
                    {
                        _endpointResponse.IsSuccess = false;
                        _endpointResponse.Message = response?.ResultMessage ?? "User not created";
                    }
                }
                catch (Exception ex)
                {
                    _globalExceptionHandler.HandleGenericException<string>(ex, "CreateUserCommand.Handle");
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = $"Error creating user: {ex.Message}";
                }
                return (IEndpointResponse<IDatabaseResult>)_endpointResponse;
            }
            #endregion
        }

    }
}
