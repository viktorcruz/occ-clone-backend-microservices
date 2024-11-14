using SharedKernel.Common.Events;
using SharedKernel.Common.Interfaces;

namespace SearchJobsService.Application.EventListeners
{
    public class SearchJobsApplyEventHandler : IEventHandler<JobSearchApplyEvent>
    {
        public Task Handle(JobSearchApplyEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
