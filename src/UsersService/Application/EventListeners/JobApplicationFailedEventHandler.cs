using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Events.JobSearch;

namespace UsersService.Application.EventListeners
{
    public class JobApplicationFailedEventHandler : IEventHandler<JobApplicationFailedEvent>
    {
        public async Task Handle(JobApplicationFailedEvent @event)
        {
            Console.WriteLine($"[User] Compensating user job history update: UserId={@event.IdApplicant}, JobId={@event.IdJob}");

            // Simulación: Eliminar historial de usuario
            await Task.Delay(500);

            Console.WriteLine($"[User] Compensation complete for UserId={@event.IdApplicant}");
        }
    }
}
