using AuthService.Application.DTO;
using AuthService.Domain.Ports.Output.Repositories;

namespace AuthService.Infrastructure.Adapters
{
    public class RenewTokenAdapter : IRenewTokenPort
    {
        #region Properties
        private readonly IUserRepository _userRepository;
        #endregion

        #region Constructor
        public RenewTokenAdapter(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        #endregion

        #region Methods
        public async Task<RenewTokenResponseDTO> RenewAsync(string emailClaim)
        {
            var user = await _userRepository.GetByEmailAsync(emailClaim);

            if (user.Details == null || !user.ResultStatus)
            {
                return new RenewTokenResponseDTO
                {
                    IdUser = 0,
                    Email = null
                };
            }

            return new RenewTokenResponseDTO
            {
                IdUser = user.Details.IdUser,
                Email = user.Details.Email,
            };
        }
        #endregion
    }
}
