using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Interface;
using UsersService.Application.Commands;
using UsersService.Application.Dto;
using UsersService.Application.Queries;

namespace UsersService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region Properties
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
        //[HttpPost]
        //public async Task<IActionResult> CreateUserAsync([FromBody] AddUserDTO userDTO)
        //{
        //    try
        //    {
        //        if (userDTO.IdRole == 0)
        //        {
        //            return BadRequest();
        //        }
        //        var command = new CreateUserCommand(userDTO);
        //        var endpointResponse = await _mediator.Send(command);
        //        if (endpointResponse.IsSuccess)
        //        {
        //            return Ok(endpointResponse);
        //        }
        //        return BadRequest(endpointResponse);

        //    }
        //    catch (Exception ex)
        //    {
        //        _globalExceptionHandler.HandleGenericException<string>(ex, "UserController.CreateUser");
        //        return StatusCode(500);
        //    }
        //}

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserRetrieveDTO>> GetUserByIdAsync(int userId)
        {
            try
            {
                var query = new GetUserByIdQuery(userId);
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
                var query = new GetAllUsersQuery();
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

        [HttpGet("search-users")]
        public async Task<IActionResult> SearchUsers([FromQuery] string? firstName, [FromQuery] string? lastName, [FromQuery] string? email)
        {
            try
            {
                var query = new SearchUsersQuery(firstName, lastName, email);
                var endpointResponse = await _mediator.Send(query);

                if (endpointResponse.IsSuccess)
                {
                    return Ok(endpointResponse);
                }
                return BadRequest(endpointResponse);
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "UserController.SearchUsers");
                return StatusCode(500);
            }
        }

        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] UserRetrieveDTO userDTO)
        {
            try
            {
                var command = new UpdateUserCommand(userDTO);
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

        [HttpPut("update-user-profile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UserProfileDTO userProfileDTO)
        {
            try
            {
                var command = new UpdateUserProfileCommand(userProfileDTO);
                var endpointResponse = await _mediator.Send(command);
                if (endpointResponse.IsSuccess)
                {
                    return Ok(endpointResponse);
                }
                return BadRequest(endpointResponse);
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "UserController.UpdateUserProfle");
                return StatusCode(500);
            }
        }

        [HttpPut("change-user-password")]
        public async Task<IActionResult> ChangeUserPassword([FromBody] UserPasswordDTO passwordDTO)
        {
            try 
            {
                var command = new ChangeUserPasswordCommand(passwordDTO);
                var endpointResponse = await _mediator.Send(command);
                if (endpointResponse.IsSuccess)
                {
                    return Ok(endpointResponse);
                }
                return BadRequest(endpointResponse);
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "UserController");
                return StatusCode(500);
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUserById(int userId)
        {
            try
            {
                var command = new DeleteUserCommand(userId);
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
