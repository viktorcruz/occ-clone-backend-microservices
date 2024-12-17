using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SearchJobsService.Application.Commands;
using SearchJobsService.Application.DTO.Commands;
using SearchJobsService.Application.Queries;
using SharedKernel.Interfaces.Exceptions;

namespace SearchJobsService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SearchJobsController : ControllerBase
    {
        #region Properties
        private readonly IMediator _mediator;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        #endregion

        #region Constructor
        public SearchJobsController(IMediator mediator, IApplicationExceptionHandler applicationExceptionHandler)
        {
            _mediator = mediator;
            _applicationExceptionHandler = applicationExceptionHandler;
        }
        #endregion

        #region Methods

        [HttpPost("apply")]
        public async Task<IActionResult> Apply([FromBody] JobApplicationRequestDTO requestDTO)
        {
            try
            {
                Console.WriteLine("[Controller] Received job application request.");

                //if (requestDTO.IdPublication == 0 || requestDTO.IdApplicant == 0 || string.IsNullOrEmpty(requestDTO.ApplicantName))
                //{
                //    return BadRequest();
                //}

                var command = new ApplyCommand(requestDTO);
                var endpointResponse = await _mediator.Send(command);
                if (endpointResponse.IsSuccess)
                {
                    Console.WriteLine("[Controller] Job application processed successfully.");

                    return Ok(endpointResponse);
                }
                Console.WriteLine("[Controller] Job application failed.");

                return BadRequest(endpointResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Controller] Error while processing job application: {ex.Message}");

                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Controller, ActionType.Apply);
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
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Controller, ActionType.Withdraw);
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
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Controller, ActionType.FetchAll);
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
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Controller, ActionType.FetchAll);
                return BadRequest(ex);
            }
        }
        #endregion
    }
}
