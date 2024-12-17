using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Events.JobSearch;

namespace SearchJobsService.Application.EventListeners.Job
{
    public class JobApplicationFailedEventHandler : IEventHandler<JobApplicationFailedEvent>
    {
        public async Task Handle(JobApplicationFailedEvent @event)
        {
            Console.WriteLine($"[SearchJobs] Compensating job application failure: JobId={@event.IdJob}, UserId={@event.IdApplicant}");
            // Simulación: Deshacer registro en base de datos
            await Task.Delay(500);

            Console.WriteLine($"[SearchJobs] Compensation complete for JobId={@event.IdJob}");
        }
    }
}
