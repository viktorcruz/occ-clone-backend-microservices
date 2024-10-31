using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PublicationsService.Aplication.Commands;
using PublicationsService.Aplication.Dto;
using PublicationsService.Aplication.Queries;
using PublicationsService.Application.Dto;
using SharedKernel.Interface;

namespace PublicationsService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PublicationController : ControllerBase
    {
        #region Properties
        private readonly IMediator _mediator;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        #endregion

        #region Constructor
        public PublicationController(IMediator mediator, IGlobalExceptionHandler globalExceptionHandler)
        {
            _mediator = mediator;
            _globalExceptionHandler = globalExceptionHandler;
        }
        #endregion

        #region Methods
        [HttpPost]
        public async Task<IActionResult> CreatePublicationAync([FromBody] PublicationDTO publicationDTO)
        {
            try
            {
                if (publicationDTO.IdUser == 0 || publicationDTO.IdRole == 0 || string.IsNullOrEmpty(publicationDTO.Title) || string.IsNullOrEmpty(publicationDTO.Description))
                {
                    return BadRequest();
                }
                var command = new CreatePublicationCommand(publicationDTO);
                var endpointResponse = await _mediator.Send(command);
                if (endpointResponse.IsSuccess)
                {
                    return Ok(endpointResponse);
                }
                return BadRequest(endpointResponse);
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "PublicationController.CreatePublication");
                return StatusCode(500); ;
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
                _globalExceptionHandler.HandleGenericException<string>(ex, "PubliactionController");
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
                _globalExceptionHandler.HandleGenericException<string>(ex, "PublicationController");
                return StatusCode(500);
            }
        }

        [HttpPut("update-publication")]
        public async Task<IActionResult> UpdatePublicationAsync(PublicationUpdateDTO retrieveDTO)
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
                _globalExceptionHandler.HandleGenericException<string>(ex, "PublicationController.UpdatePublicationAsync");
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
                _globalExceptionHandler.HandleGenericException<string>(ex, "PublicationController");
                return StatusCode(500);
            }
        }
        #endregion
    }
}
