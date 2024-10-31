using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Interface;

namespace SearchJobsServcie.Controllers
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
        [HttpGet]
        public async Task<IActionResult> GetAllJobs()
        {
            return Ok("Alles Ok");
        }
        #endregion
    }
}
