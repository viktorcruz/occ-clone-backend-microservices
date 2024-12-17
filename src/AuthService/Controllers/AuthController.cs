using AuthService.Application.Commands;
using AuthService.Application.DTO;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Interfaces.Exceptions;

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
        private readonly IConfirRegisterUserCase _confirRegisterUserCase;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        #endregion

        #region Constructor
        public AuthController(
            ILoginUseCase loginUseCase,
            IRegisterUseCase registerUseCase,
            IRenewTokenUseCase renewTokenUseCase,
            IConfirRegisterUserCase confirRegisterUserCase,
            IApplicationExceptionHandler applicationExceptionHandler
            )
        {
            _loginUseCase = loginUseCase;
            _registerUseCase = registerUseCase;
            _renewTokenUseCase = renewTokenUseCase;
            _confirRegisterUserCase = confirRegisterUserCase;
            _applicationExceptionHandler = applicationExceptionHandler;
        }
        #endregion

        #region Methods
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            try
            {
                //if (command == null || string.IsNullOrEmpty(command.Email) || string.IsNullOrEmpty(command.Password) || command.Password.Length < 8)
                //{
                //    return BadRequest();
                //}
                var response = await _loginUseCase.Execute(command);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Controller, ActionType.Login);
                return Unauthorized("Invalid credentials");
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Controller, ActionType.Login);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            try
            {
                //if (command == null || command.IdRole <= 0
                //    || string.IsNullOrEmpty(command.Email)
                //    || string.IsNullOrEmpty(command.Password)
                //    || command.Password.Length < 8)
                //{
                //    return BadRequest();
                //}
                var response = await _registerUseCase.Execute(command);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Controller, ActionType.Register);
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Controller, ActionType.Register);
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize]
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
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Controller, ActionType.Renew);
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Controller, ActionType.Renew);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("confirm-registration")]
        public async Task<IActionResult> ConfirmUserRegistration([FromBody] ConfirmRegistrationDTO confirmDTO)
        {
            try
            {
                //if (string.IsNullOrEmpty(email)) { return BadRequest(); }
                var command = new ConfirmRegisterCommand(confirmDTO);
                var response = await _confirRegisterUserCase.Execute(command);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Controller, ActionType.Update);
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Controller, ActionType.Update);
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion
    }
}