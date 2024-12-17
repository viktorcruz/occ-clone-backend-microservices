using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Events.JobSearch;

namespace SearchJobsService.Application.EventListeners.Job
{
    public class SearchJobsApplyEventHandler : IEventHandler<JobSearchApplyEvent>
    {
        public Task Handle(JobSearchApplyEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
