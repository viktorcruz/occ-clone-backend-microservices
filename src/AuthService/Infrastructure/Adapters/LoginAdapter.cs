using AuthService.Application.Commands;
using AuthService.Application.DTO;
using AuthService.Domain.Ports.Output.Repositories;
using AuthService.Domain.Ports.Output.Services;
using AuthService.Infrastructure.Security;

namespace AuthService.Infrastructure.Adapters
{
    public class LoginAdapter : ILoginPort
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ITokenService _tokenService;

        public LoginAdapter(IUserRepository userRepository, IRoleRepository roleRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _tokenService = tokenService;
        }

        public async Task<LoginUserResponseDTO> LoginAsync(LoginCommand command)
        {
            var existingUser = await _userRepository.GetByEmailAsync(command.Email);
            
            if(existingUser.Details == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }
            
            bool isMatch = PasswordHasher.VerifyPassword(command.Password, existingUser.Details.PasswordHash);
            
            if(!isMatch)
            {
                throw new UnauthorizedAccessException("Invalid password");
            }

            var user = await _userRepository.GetUserByCredentialsAsync(command.Email);

            if (user.Details == null || !user.ResultStatus)
            {
                throw new UnauthorizedAccessException("Invalid credentials, check your email or password");
            }

            var role = await _roleRepository.GetRoleByIdAsync(user.Details.IdRole);

            if (role.Details == null || !role.ResultStatus)
            {
                throw new InvalidOperationException($"Role with Id not found: {user.Details.IdRole}");
            }

            var userDetails = new LoginUserResponseDTO
            {
                IdUser = user.Details.IdUser,
                IdRole = user.Details.IdRole,
                Email = user.Details.Email,
            };

            var token = _tokenService.GenerateAccessToken(userDetails);

            userDetails.Token = token;
            return userDetails;
        }
    }
}
