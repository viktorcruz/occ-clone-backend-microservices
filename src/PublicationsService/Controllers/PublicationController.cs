using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PublicationsService.Aplication.Commands;
using PublicationsService.Aplication.DTO;
using PublicationsService.Aplication.Queries;
using PublicationsService.Application.DTO;
using PublicationsService.Saga;
using SharedKernel.Interfaces.Exceptions;

namespace PublicationsService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PublicationController : ControllerBase
    {
        #region Properties
        private readonly IMediator _mediator;
        private readonly SagaOrchestrator _sagaOrchestrator;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        #endregion

        #region Constructor
        public PublicationController(SagaOrchestrator sagaOrchestrator, IMediator mediator, IApplicationExceptionHandler applicationExceptionHandler)
        {
            _sagaOrchestrator = sagaOrchestrator;
            _mediator = mediator;
            _applicationExceptionHandler = applicationExceptionHandler;
        }
        #endregion

        #region Methods
        [HttpPost]
        public async Task<IActionResult> CreatePublicationAync([FromBody] PublicationDTO publicationDTO)
        {
            try
            {
                //if (publicationDTO.IdUser == 0 || publicationDTO.IdRole == 0 || string.IsNullOrEmpty(publicationDTO.Title) || string.IsNullOrEmpty(publicationDTO.Description))
                //{
                //    return BadRequest();
                //}
                var command = new CreatePublicationCommand(publicationDTO);
                var cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = cancellationTokenSource.Token;
                await _sagaOrchestrator.ExecuteAsync(command, cancellationToken);
                //var endpointResponse = await _mediator.Send(command);
                //if (endpointResponse.IsSuccess)
                //{
                //    return Ok(endpointResponse);
                //}
                //return BadRequest(endpointResponse);
                return Ok("Publication created successfully");
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Controller, ActionType.Insert);
                return BadRequest($"Saga failed: {ex.Message}");
            }
        }

        [HttpGet("{publicationId}")]
        public async Task<IActionResult> GetPublicationByIdAsync(int publicationId)
        {
            try
            {
                var query = new GetPublicationByIdQuery(publicationId);
                var endpointResponse = await _mediator.Send(query);
                if (endpointResponse.IsSuccess)
                {
                    return Ok(endpointResponse);
                }
                return BadRequest(endpointResponse);
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Controller, ActionType.Get);
                return StatusCode(500);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllPublicationsAsync()
        {
            try
            {
                var query = new GetAllPulicationsQuery();
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
                return StatusCode(500);
            }
        }

        [HttpPut("update-publication")]
        public async Task<IActionResult> UpdatePublicationAsync([FromBody] PublicationUpdateDTO retrieveDTO)
        {
            try
            {
                var command = new UpdatePublicationCommand(retrieveDTO);
                var endpointResponse = await _mediator.Send(command);

                if (endpointResponse.IsSuccess)
                {
                    return Ok(endpointResponse);
                }

                return BadRequest(endpointResponse);
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Controller, ActionType.Update);
                return StatusCode(500);
            }
        }

        [HttpDelete("{publicationId}")]
        public async Task<IActionResult> DeletePublicationAsync(int publicationId)
        {
            try
            {
                var query = new DeletePublicationCommand(publicationId);
                var endpointResponse = await _mediator.Send(query);
                if (endpointResponse.IsSuccess)
                {
                    return Ok(endpointResponse);
                }
                return BadRequest(endpointResponse);
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Controller, ActionType.Delete);
                return StatusCode(500);
            }
        }
        #endregion
    }
}
