//using MediatR;
//using SharedKernel.Interface;
//using UsersService.Application.Dto;

//namespace UsersService.Application.Commands
//{
//    public class CreateUserCommand : IRequest<IEndpointResponse<IDatabaseResult>>
//    {
//        #region Properties
//        public int IdRole { get; set; }
//        public string FirstName { get; set; }
//        public string LastName { get; set; }
//        public string Email { get; set; }
//        #endregion

//        #region Constructor
//        public CreateUserCommand(AddUserDTO userDTO)
//        {
//            IdRole = userDTO.IdRole;
//            FirstName = userDTO.FirstName;
//            LastName = userDTO.LastName;
//            Email = userDTO.Email;
//        }
//        #endregion
//    }
//}
