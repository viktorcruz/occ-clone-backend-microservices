using Microsoft.AspNetCore.Http;
namespace SharedKernel.Interfaces.Service
{
    public interface IPerformedByService
    {
        string GetPerformedBy();
        Task Invoke(HttpContext context);
    }
}
