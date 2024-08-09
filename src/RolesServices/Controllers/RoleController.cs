using MediatR;
using Microsoft.AspNetCore.Mvc;
using RolesServices.Aplication.Commands;
using RolesServices.Aplication.Dto;
using RolesServices.Aplication.Queries;
using SharedKernel.Common.Response;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

namespace RolesServices.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        #region Properties
        private readonly IMediator _mediator;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        #endregion

        #region Constructor
        public RoleController(IMediator mediator, IGlobalExceptionHandler globalExceptionHandler)
        {
            _mediator = mediator;
            _globalExceptionHandler = globalExceptionHandler;
        }
        #endregion

        #region Methods
        [HttpPost]
        public async Task<IActionResult> CreateRoleAsync([FromBody] AddRoleDTO addRoleDTO)
        {
            try
            {
                var command = new CreateRoleCommand.TaskCommand(addRoleDTO);
                var endpointResponse = await _mediator.Send(command);
                if (endpointResponse != null)
                {
                    return Ok(endpointResponse);
                }
                return BadRequest(endpointResponse);

            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "RoleController.CreateRoleAsync");
                return StatusCode(500);
            }
        }

        [HttpGet("{roleId}")]
        public async Task<ActionResult<RoleDTO>> GetRoleByIdAsync(int roleId)
        {
            try
            {
                var query = new GetRoleByIdQuery.TaskQuery(roleId);
                var endpointResponse = await _mediator.Send(query);
                if (endpointResponse != null)
                {
                    return Ok(endpointResponse);
                }
                return BadRequest(endpointResponse);
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "RoleController.GetRoleByIdASync");
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var query = new GetAllRoleQuery();
                var endpointResponse = await _mediator.Send(query);
                if (endpointResponse != null)
                {
                    return Ok(endpointResponse);
                }
                return BadRequest(endpointResponse);
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "RoleController.GetAllRoles");
                return StatusCode(500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRolesAsync([FromBody] RoleDTO roleDTO)
        {
            try
            {
                var command = new UpdateRoleCommand.TaskCommand(roleDTO);
                var endpointResponse = await _mediator.Send(command);
                if (endpointResponse != null)
                {
                    return Ok(endpointResponse);
                }
                return BadRequest(endpointResponse);
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "RoleController.UpdateRolesAsync");
                return StatusCode(500);
            }
        }

        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRoleAsync(int roleId)
        {
            try
            {
                var command = new DeleteRoleCommand.TaskCommand(roleId);
                var endpointResopnse = await _mediator.Send(command);
                if (endpointResopnse != null)
                {
                    return Ok(endpointResopnse);
                }
                return BadRequest(endpointResopnse);
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "RoleController.DeleteRoleAsync");
                return StatusCode(500);
            }
        }
        #endregion
    }
}
