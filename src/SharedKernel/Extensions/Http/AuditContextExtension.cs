using Microsoft.AspNetCore.Http;

namespace SharedKernel.Extensions.Http
{
    public static class AuditContextExtension
    {
        public static string GtePerformedBy(this IHttpContextAccessor contextAccessor)
        {
            return (string?)contextAccessor.HttpContext?.Items["PerformedBy"] ?? "Unknown";
        }
    }
}
