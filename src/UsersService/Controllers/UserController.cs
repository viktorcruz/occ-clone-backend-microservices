using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Interface;
using UsersService.Application.Commands;
using UsersService.Application.Dto;
using UsersService.Application.Queries;

namespace UsersService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region Porperties
        private readonly IMediator _mediator;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        #endregion

        #region Constructor
        public UserController(IMediator mediator, IGlobalExceptionHandler globalExceptionHandler)
        {
            _mediator = mediator;
            _globalExceptionHandler = globalExceptionHandler;
        }
        #endregion

        #region Methods
        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] AddUserDTO userDTO)
        {
            try
            {
                var command = new CreateUserCommand.TaskCommand(userDTO);
                var endpointResponse = await _mediator.Send(command);
                if (endpointResponse != null)
                {
                    return Ok(endpointResponse);
                }
                return BadRequest(endpointResponse);

            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "UserController.CreateUser");
                return StatusCode(500);
            }
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserRetrieveDTO>> GetUserByIdAsync(int userId)
        {
            try
            {
                var query = new GetUserByIdQuery.TaskQuery(userId);
                var endpointResponse = await _mediator.Send(query);
                if (endpointResponse.IsSuccess)
                {
                    return Ok(endpointResponse);
                }
                return BadRequest(endpointResponse);
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "UserController.GetUserById");
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var query = new GetAllUserQuery();
                var endpointResponse = await _mediator.Send(query);
                if (endpointResponse.IsSuccess)
                {
                    return Ok(endpointResponse);
                }
                return BadRequest(endpointResponse);
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "UserController.GetAllUsers");
                return StatusCode(500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UserRetrieveDTO userDTO)
        {
            try
            {
                var command = new UpdateUserCommand.TaskCommand(userDTO);
                var endpointResponse = await _mediator.Send(command);
                if (endpointResponse.IsSuccess)
                {
                    return Ok(endpointResponse);
                }
                return BadRequest(endpointResponse);
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "UserController.UpdateUser");
                return StatusCode(500);
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUserById(int userId)
        {
            try
            {
                var command = new DeleteUserCommand.TaskCommand(userId);
                var endpointResponse = await _mediator.Send(command);
                if (endpointResponse != null)
                {
                    return Ok(endpointResponse);
                }
                return BadRequest(endpointResponse);
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "UserController.DeleteUserById");
                return StatusCode(500);
            }
        }
        #endregion
    }
}
