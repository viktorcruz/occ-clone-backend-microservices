using Ocelot.Middleware;
using Ocelot.Multiplexer;

namespace OrquestadorService.Aggregator
{
    public class UserRolesAggregator : IDefinedAggregator
    {
        public Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
        {
            throw new NotImplementedException();
        }
    }
}
