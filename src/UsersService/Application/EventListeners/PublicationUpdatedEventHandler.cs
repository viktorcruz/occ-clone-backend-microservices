using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Events.JobSearch;
using SharedKernel.Events.Publication;
using SharedKernel.Events.User;
using SharedKernel.Extensions.Routing;
using UsersService.Application.DTO;
using UsersService.Domain.Interface;

namespace UsersService.Application.EventListeners
{
    public class PublicationUpdatedEventHandler : IEventHandler<PublicationUpdatedEvent>
    {
        private readonly IUserDomain _userDomain;
        private readonly IEventPublisherService _eventPublisherService;

        public PublicationUpdatedEventHandler(IUserDomain userDomain, IEventPublisherService eventPublisherService)
        {
            _userDomain = userDomain;
            _eventPublisherService = eventPublisherService;
        }

        public async Task Handle(PublicationUpdatedEvent @event)
        {
            try
            {
                Console.WriteLine($"[UserService] Updating user job history for PublicationId={@event.IdPublication}");

                // Construir el DTO completo para el usuario
                var userProfileUpdate = new UserProfileDTO
                {
                    IdUser = @event.IdUser, // Relación obtenida de otros servicios si es necesario
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "johndoe@example.com"
                };

                // Actualizar el perfil completo del usuario
                var response = await _userDomain.UpdateUserProfileAsync(userProfileUpdate);

                if (response == null || !response.ResultStatus)
                {
                    throw new Exception($"User not found for update: {userProfileUpdate.IdUser}");
                }

                Console.WriteLine("[UserService] Successfully updated user profile.");

                // Publicar un evento indicando que el historial del usuario se actualizó
                var userUpdatedEvent = new UserUpdatedEvent
                {
                    IdUser = userProfileUpdate.IdUser,
                    UpdatedAt = DateTime.UtcNow
                };

                await _eventPublisherService.PublishEventAsync(
                    entityName: "User",
                    operationType: "UPDATE",
                    success: true,
                    performedBy: "System",
                    reason: "User profile updated successfully",
                    additionalData: userUpdatedEvent,
                    exchangeName: PublicationExchangeNames.User.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Update_Success.ToRoutingKey());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserService] Error updating user job history: {ex.Message}");

                // Publicar evento de compensación
                var failedEvent = new JobApplicationFailedEvent
                {
                    IdJob = @event.IdPublication,
                    IdApplicant = @event.IdUser, // Obtener el Id del contexto o evento original
                    Reason = ex.Message,
                    FailedAt = DateTime.UtcNow
                };

                await _eventPublisherService.PublishEventAsync(
                    entityName: "Job",
                    operationType: "APPLY",
                    success: false,
                    performedBy: "System",
                    reason: ex.Message,
                    additionalData: failedEvent,
                    exchangeName: PublicationExchangeNames.Job.ToExchangeName(),
                    routingKey: PublicationRoutingKeys.Apply_Error.ToRoutingKey());
            }
        }
    }
}
