using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Events.JobSearch;

namespace UsersService.Application.EventListeners
{
    public class SearchJobsApplyEventHandler : IEventHandler<JobSearchApplyEvent>
    {
        public Task Handle(JobSearchApplyEvent @event)
        {
            Console.WriteLine($"User apply: {@event.IdApplicant}, publication: {@event.IdPublication}");
            // Lógica adicional para el evento
            return Task.CompletedTask;
        }
    }
}
