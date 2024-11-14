using SharedKernel.Common.Events;
using SharedKernel.Common.Extensions;
using SharedKernel.Common.Interfaces;
using UsersService.Infrastructure.Interface;

namespace UsersService.Saga
{
    public class CompensationActions
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

        public async Task CompensateCreateUserAsync(UserSagaContext sagaState)
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
