using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Common.Messaging;


namespace SharedKernel.Extensions.Router
{
    public static class EventRouterExtensions
    {
        public static IServiceCollection AddEventRouter(this IServiceCollection services)
        {
            services.AddSingleton<EventRouter>();
            return services;
        }

        public static WebApplication UseEventRouter(this WebApplication app)
        {
            var eventRouter = app.Services.GetRequiredService<EventRouter>();

            return app;
        }
    }
}
