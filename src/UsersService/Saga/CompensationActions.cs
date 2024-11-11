//using UsersService.Infrastructure.Interface;

//namespace UsersService.Saga
//{
//    public class CompensationActions
//    {
//        private readonly IUserRepository _userRepository;
//        private readonly IPublicationService _publicationService;
//        private readonly ISearchJobsService _searchJobsService;

//        public CompensationActions(
//            IUserRepository userRepository,
//            IPublicationService publicationService,
//            ISearchJobsService searchJobsService
//        )
//        {
//            _userRepository = userRepository;
//            _publicationService = publicationService;
//            _searchJobsService = searchJobsService;
//        }

//        public async Task CompensateCreateUserAsync(UserSagaContext sagaState)
//        {
//            if (sagaState.IsJobSearchUpdated)
//            {
//                // Revertir la actualización en SearchJobsService
//                await _searchJobsService.RevertJobSearchUpdateAsync(sagaState.IdUser);
//                sagaState.IsJobSearchUpdated = false;
//            }

//            if (sagaState.IdPublication != 0)
//            {
//                // Revertir la creación de la publicación
//                await _publicationService.DeletePublicationAsync(sagaState.IdPublication);
//                sagaState.IdPublication = 0;
//            }

//            if (sagaState.IdUser != 0)
//            {
//                // Revertir la creación del usuario
//                await _userRepository.DeleteUserAsync(sagaState.IdUser);
//                sagaState.IdUser = 0;
//            }
//        }
//    }

//}
