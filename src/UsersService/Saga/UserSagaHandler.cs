using UsersService.Application.Commands;
using UsersService.Infrastructure.Interface;

namespace UsersService.Saga
{
    public class UserSagaHandler
    {
        //private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        //private readonly IPublicationService _publicationService;
        //private readonly ISearchJosbService _searchJobsService;
        private readonly UserSagaState _sagaState;
        private readonly CompensationActions _compensationActions;


        public UserSagaHandler(
            IUserRepository userRepository,
            CompensationActions compensationActions
            )
        {
            _userRepository = userRepository;
            _compensationActions = compensationActions;
            _sagaState = new UserSagaState();
        }

        public async Task Handle(CreateUserCommand command)
        {
            try
            {
                // Step 1: Create users
                var dto = new Application.Dto.AddUserDTO
                {
                    IdRole = command.IdRole,
                    FirstName = command.FirstName,
                    LastName = command.LastName,
                    Email = command.Email,
                };
                var result = await _userRepository.CreateUserAsync(dto);
                _sagaState.IdUser = result.AffectedRecordId;
                _sagaState.CurrentState = "UserCreated";

                // Step 2: Create publications


                // Step 3: Update job search service


                _sagaState.IsSagaComplete = true;
            }
            catch (Exception ex)
            {
                // Something has gone wrong
                _sagaState.IsSagaComplete = false;
                await _compensationActions.CompensateCreateUserAsync(_sagaState);
                throw;
            }
        }
    }
}
