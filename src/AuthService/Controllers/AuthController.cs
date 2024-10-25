using AuthService.Application.Commands;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region Properties
        private readonly ILoginUseCase _loginUseCase;
        private readonly IRegisterUseCase _registerUseCase;
        private readonly IRenewTokenUseCase _renewTokenUseCase;
        #endregion

        #region Constructor
        public AuthController(ILoginUseCase loginUseCase, IRegisterUseCase registerUseCase, IRenewTokenUseCase renewTokenUseCase)
        {
            _loginUseCase = loginUseCase;
            _registerUseCase = registerUseCase;
            _renewTokenUseCase = renewTokenUseCase;
        }
        #endregion

        #region Methods
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            try
            {
                if (command == null || string.IsNullOrEmpty(command.Email) || string.IsNullOrEmpty(command.Password) || command.Password.Length < 8)
                {
                    return BadRequest();
                }
                var response = await _loginUseCase.Execute(command);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            try
            {
                if (command == null || command.IdRole <= 0
                    || string.IsNullOrEmpty(command.Email)
                    || string.IsNullOrEmpty(command.Password)
                    || command.Password.Length < 8)
                {
                    return BadRequest();
                }
                var response = await _registerUseCase.Execute(command); 
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("re-new")]
        public async Task<IActionResult> RenewToken([FromBody] RenewCommand command)
        {
            try
            {
                if (command == null || string.IsNullOrEmpty(command.RefreshToken))
                {
                    return BadRequest();
                }
                var response = await _renewTokenUseCase.Execute(command);
                return Ok(response);

            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion
    }
}