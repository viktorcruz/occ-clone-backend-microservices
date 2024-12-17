using SharedKernel.Interfaces.Service;

namespace SharedKernel.Services
{
    public class CorrelationService : ICorrelationService
    {
        public string GetCorrelationId()
        {
            return NLog.GlobalDiagnosticsContext.Get("IdCorrelation");
        }
    }
}
