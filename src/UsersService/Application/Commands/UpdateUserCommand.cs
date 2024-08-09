using MediatR;
using SharedKernel.Common.Response;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using UsersService.Application.Dto;
using UsersService.Domain.Interface;

namespace UsersService.Application.Commands
{
    public class UpdateUserCommand
    {
        public class TaskCommand : IRequest<IEndpointResponse<IDatabaseResult>>
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

        public class UpdateUserCommandHandler : IRequestHandler<TaskCommand, IEndpointResponse<IDatabaseResult>>
        {
            #region Properties
            private readonly IUserDomain _usersDomain;
            private readonly IGlobalExceptionHandler _globalExceptionHandler;
            private readonly IEndpointResponse<IDatabaseResult> _endpointResponse;
            #endregion

            #region Constructor
            public UpdateUserCommandHandler(IUserDomain usersDomain, IGlobalExceptionHandler globalExceptionHandler, IEndpointResponse<IDatabaseResult> endpointResponse)
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
                    var user = new UserRetrieveDTO
                    {
                        IdUser = request.IdUser,
                        IdRole = request.IdRole,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Email = request.Email,
                    };

                    var response = await _usersDomain.UpdateUserAsync(user);
                    _endpointResponse.Result = response;

                    if (response != null && response.ResultStatus)
                    {
                        _endpointResponse.IsSuccess = true;
                        _endpointResponse.Message = "User updated sucessful";
                    }
                    else
                    {
                        _endpointResponse.IsSuccess = false;
                        _endpointResponse.Message = response?.ResultMessage ?? "User not found";
                    }
                }
                catch (Exception ex)
                {
                    _globalExceptionHandler.HandleGenericException<string>(ex, "UpdateUserCommand.Handle");
                    _endpointResponse.IsSuccess = false;
                    _endpointResponse.Message = $"Error updateing user: {ex.Message}";
                }
                return _endpointResponse;
            }
            #endregion
        }
    }
}
