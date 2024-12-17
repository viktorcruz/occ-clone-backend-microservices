using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Events.JobSearch;

namespace PublicationsService.Application.EventListeners
{
    public class JobApplicationFailedEventHandler : IEventHandler<JobApplicationFailedEvent>
    {
        public async Task Handle(JobApplicationFailedEvent @event)
        {
            Console.WriteLine($"[Publication] Compensating publication update: JobId={@event.IdJob}");

            // Simulación: Deshacer conteo de postulantes
            await Task.Delay(500);

            Console.WriteLine($"[Publication] Compensation complete for JobId={@event.IdApplicant}");
        }
    }
}
