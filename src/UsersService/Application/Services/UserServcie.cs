//using UsersService.Application.Dto;
//using UsersService.Application.Services.Interface;
//using UsersService.Domain.Entity;
//using UsersService.Infrastructure.Interface;

//namespace UsersService.Application.Services
//{
//    public class UserServcie : IUserService
//    {
//        #region Properties
//        private readonly IUserRepository _userRepository;
//        #endregion

//        #region Constructor
//        public UserServcie(IUserRepository userRepository)
//        {
//            _userRepository = userRepository;
//        }
//        #endregion

//        #region Methods
//        public async Task<Guid> CreateUserAsync(AddUserDTO addUserDTO)
//        {
//            //throw new NotImplementedException();
//            // TODO: 
//            var user = new AddUserDTO
//            {
//                IdRole = addUserDTO.IdRole,
//                FirstName = addUserDTO.FirstName,
//                LastName = addUserDTO.LastName,
//                Email = addUserDTO.Email,
//            };

//            var response = await _userRepository.CreateUserAsync(user);
//            return Guid.NewGuid();
//        }

//        public Task DeleteUserAsync(Guid userId)
//        {
//            throw new NotImplementedException();
//            // TODO: 
//        }

//        public Task<UserEntity> GetUserByIdAsync(Guid userId)
//        {
//            throw new NotImplementedException();
//            // TODO:
//        }
//        #endregion
//    }
//}
