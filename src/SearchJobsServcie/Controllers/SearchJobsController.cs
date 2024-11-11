using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SearchJobsService.Application.Commands;
using SearchJobsService.Application.Dto;
using SearchJobsService.Application.Queries;
using SharedKernel.Interface;

namespace SearchJobsService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SearchJobsController : ControllerBase
    {
        #region Properties
        private readonly IMediator _mediator;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        #endregion

        #region Constructor
        public SearchJobsController(IMediator mediator, IGlobalExceptionHandler globalExceptionHandler)
        {
            _mediator = mediator;
            _globalExceptionHandler = globalExceptionHandler;
        }
        #endregion

        #region Methods

        [HttpPost("apply")]
        public async Task<IActionResult> Apply([FromBody] JobApplicationRequestDTO requestDTO)
        {
            try
            {

                if (requestDTO.IdPublication == 0 || requestDTO.IdApplicant == 0 || string.IsNullOrEmpty(requestDTO.ApplicantName))
                {
                    return BadRequest();
                }

                var command = new ApplyCommand(requestDTO);
                var endpointResponse = await _mediator.Send(command);
                if (endpointResponse.IsSuccess)
                {
                    return Ok(endpointResponse);
                }
                return BadRequest(endpointResponse);
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "SearchJobsController");
                return BadRequest(ex);
            }
        }

        [HttpPost("witdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawApplicationRequestDTO requestDTO)
        {
            try
            {
                var query = new WithdrawCommand(requestDTO);
                var endpointResponse = await _mediator.Send(query);
                if (endpointResponse.IsSuccess)
                {
                    return Ok(endpointResponse);
                }
                return BadRequest(endpointResponse);
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "SearchJobsController");
                return BadRequest(ex);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            try
            {
                var query = new SearchJobsQuery(keyword);
                var endpointResponse = await _mediator.Send(query);

                if (endpointResponse.IsSuccess)
                {
                    return Ok(endpointResponse);
                }
                return BadRequest($"Search: {keyword}");
            }
            catch(Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "SearchJobsController");
                return BadRequest(ex);
            }
        }

        [HttpGet("applications")]
        public async Task<IActionResult> Applications(int applicantId)
        {
            try
            {
                if (applicantId == 0)
                {
                    return BadRequest();
                }
                var query = new ApplicationQuery(applicantId);
                var endpointResponse = await _mediator.Send(query);
                if (endpointResponse.IsSuccess)
                {
                    return Ok(endpointResponse);
                }
                return BadRequest(endpointResponse);
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "SearchJobsController");
                return BadRequest(ex);
            }
        }
        #endregion
    }
}
