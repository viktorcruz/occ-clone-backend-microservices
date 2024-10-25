using AuthService.Application.Commands;
using AuthService.Application.DTO;
using AuthService.Domain.Entities;
using AuthService.Domain.Ports.Output.Repositories;
using AuthService.Infrastructure.Security;

namespace AuthService.Infrastructure.Adapters
{
    public class RegisterAdapter : IRegisterPort
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly IUserRepository _userRepository;

        public RegisterAdapter(IRegisterRepository registerRepository, IUserRepository userRepository)
        {
            _registerRepository = registerRepository;
            _userRepository = userRepository;
        }

        public async Task<RegisterUserDTO> RegisterAsync(RegisterCommand command)
        {
            var existingUser = await _userRepository.GetByEmailAsync(command.Email);

            if (existingUser.Details != null || existingUser.ResultStatus)
            {
                throw new Exception("Email alredy exists, please select anther email");
            }

            string hashedPassword = PasswordHasher.HashPassword(command.Password);
       
            bool isMatch = PasswordHasher.VerifyPassword(command.Password, hashedPassword);

            if (!isMatch)
            {
                throw new Exception("Password incorrect.");
            }

            var newUser = new RegisterEntity
            {
                IdRole = command.IdRole,
                FirstName = command.FirstName,
                LastName = command.LastName,
                Email = command.Email,
                PasswordHash = hashedPassword
            };

            var response = await _registerRepository.AddAsync(newUser);

            if (!response.ResultStatus && response.AffectedRecordId == 0)
            {
                throw new Exception(response.ResultStatus.ToString());
            }

            var userResponse = new RegisterUserDTO
            {
                IdRole = newUser.IdRole,
                FirstName = command.FirstName,
                LastName = command.LastName,
                Email = newUser.Email,
                Password = "*******"
            };

            return userResponse;
        }
    }
}
