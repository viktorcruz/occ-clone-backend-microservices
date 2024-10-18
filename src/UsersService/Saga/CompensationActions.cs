using UsersService.Infrastructure.Interface;

namespace UsersService.Saga
{
    public class CompensationActions
    {
        #region Properties
        private readonly IUserRepository _userRepository;
        //private readonly IUserService _userService;
        //private readonly IPublicationService _publicationService;
        //private readonly ISearchJobsServcie _searchJobsServcie;
        #endregion

        #region Constructor
        public CompensationActions(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        #endregion

        #region Methods
        public async Task CompensateCreateUserAsync(UserSagaState sagaState)
        {
            // Step 3
            if (sagaState.IsJobSearchUpdated)
            {
                // TODO:
                //await _searchJobsService.DeletePublicationAsync(sagaState.IdPublication);
            }

            // Step 2
            if (sagaState.IdPublication != 0)
            {
                // TOTO:
                //await _publicationsService.DeletePublicationAsync(sagaState.IdPublication);
            }

            // Step 1
            if (sagaState.IdUser != 0)
            {
                await _userRepository.DeleteUserAsync(sagaState.IdUser);
            }
        }
        #endregion
    }
}
