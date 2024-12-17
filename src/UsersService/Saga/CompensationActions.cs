using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Events.JobSearch;
using SharedKernel.Extensions.Routing;
using UsersService.Infrastructure.Interface;
using UsersService.Saga.Interfaces;

namespace UsersService.Saga
{
    public class CompensationActions : ICompensationActions
    {
        private readonly IUserRepository _userRepository;
        private readonly IEventPublisherService _eventPublisherService;
        public CompensationActions(
            IUserRepository userRepository, 
            IEventPublisherService eventPublisherService
        )
        {
            _userRepository = userRepository;
            _eventPublisherService = eventPublisherService;
        }

        public async Task CompensateCreateUserAsync(IUserSagaContext sagaState)
        {
            if (sagaState.IsJobSearchUpdated)
            {
                // publicar evento de reversion para searchJobsService
                var jobSearchReverEvent = new JobSearchRevertEvent
                {
                    IdUser = sagaState.IdUser
                };

                await _eventPublisherService.PublicEventAsync(
                    exchangeName: PublicationExchangeNames.Job.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Revert.ToRoutingKey(),
                    eventMessage: jobSearchReverEvent
                    );
            }

            if (sagaState.IdPublication != 0)
            {
                // Revertir la creación de la publicación
                //await _publicationService.DeletePublicationAsync(sagaState.IdPublication);
                sagaState.IdPublication = 0;
            }

            if (sagaState.IdUser != 0)
            {
                // Revertir la creación del usuario
                await _userRepository.DeleteUserAsync(sagaState.IdUser);
                sagaState.IdUser = 0;
            }
        }
    }
}
