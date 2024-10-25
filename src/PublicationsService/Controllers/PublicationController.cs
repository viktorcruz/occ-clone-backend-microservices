using MediatR;
using Microsoft.AspNetCore.Mvc;
using PublicationsService.Aplication.Commands;
using PublicationsService.Aplication.Dto;
using SharedKernel.Interface;

namespace PublicationsService.Controllers
{
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
                if (publicationDTO.IdUser == 0)
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
        #endregion
    }
}
