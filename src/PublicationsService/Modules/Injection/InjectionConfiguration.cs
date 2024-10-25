using PublicationsService.Domain.Core;
using PublicationsService.Domain.Events;
using PublicationsService.Domain.Interface;
using PublicationsService.Infrastructure.Interface;
using PublicationsService.Infrastructure.Messaging;
using PublicationsService.Infrastructure.Repository;
using PublicationsService.Persistence.Data;
using PublicationsService.Persistence.Interface;
using SharedKernel.Common;
using SharedKernel.Common.Exceptions;
using SharedKernel.Common.Response;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

namespace PublicationsService.Modules.Injection
{
    public static class InjectionConfiguration
    {
        public static IServiceCollection AddCustomInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<RabbitMQConnection>();
            services.AddSingleton<EventBusRabbitMQ>();
            services.AddSingleton<IEventBus, EventBusRabbitMQ>();
            services.AddTransient<EntityOperationEvent>();
            services.AddSingleton<IDapperExecutor, DapperExecutor>();
            services.AddSingleton<IEventLogRepository, EventLogRepository>();
            services.AddTransient<IPublicationDomain, PublicationDomain>();
            services.AddTransient<IPublicationRepository, PublicationRepository>();
            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            services.AddSingleton<IGlobalExceptionHandler, GlobalExceptionHandler>();
            services.AddTransient(typeof(IEndpointResponse<>), typeof(EndpointResponse<>));
            services.AddTransient<IDatabaseResult, DatabaseResult>();

            return services;
        }
    }
}
