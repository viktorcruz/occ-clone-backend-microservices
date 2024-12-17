using SearchJobsService.Domain.Core;
using SearchJobsService.Domain.Interface;
using SearchJobsService.Infrastructure.Interface;
using SearchJobsService.Infrastructure.Repository;
using SharedKernel.Common.Messaging;
using SharedKernel.Common.Repositories;
using SharedKernel.Common.Response;
using SharedKernel.Common.Responses;
using SharedKernel.Data;
using SharedKernel.Audit;
using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Common.Interfaces.Persistence;
using SearchJobsService.Application.EventListeners.User;
using SharedKernel.Services;
using SharedKernel.Dapper;
using SharedKernel.Events.User;
using SharedKernel.Exceptions.Application;
using SharedKernel.Interfaces.Exceptions;
using SharedKernel.Interfaces.Response;
using SharedKernel.Interfaces.Service;
using SharedKernel.Interfaces.Audit;
using SharedKernel.Interfaces.Dapper;

namespace SearchJobsService.Modules.Injection
{
    public static class Injection
    {
        public static IServiceCollection AddCustomInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRabbitMQ(configuration);
            services.AddRepositories();
            services.AddDomainServices();
            services.AddEventHandler();
            services.AddCommonServices(configuration);
            //services.AddSingleton<IEventPublisherService, SagaOrchestrator>();

            return services;
        }

        private static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<RabbitMQSettings>();
            services.AddSingleton<RabbitMQConnection>();
            services.AddSingleton<IAsyncEventBus, RabbitMQEventBus>();
            services.AddSingleton<EventRouter>();

            return services;
        }

        private static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddTransient<ISearchJobsDomain, SearchJobsDomain>();
            services.AddTransient<IAuditEventFactory, AuditEventFactory>();

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IEventLogStorage, EventLogStorage>();
            services.AddTransient<ISearchJobsRepository, SearchJobsRepository>();

            return services;
        }

        private static IServiceCollection AddEventHandler(this IServiceCollection services)
        {
            services.AddScoped<IEventHandler<UserCreatedEvent>, UserCreatedEventHandler>();
            services.AddScoped<IEventHandler<UserCreationSucceededEvent>, UserCreationSucceededEventHandler>();
            services.AddScoped<IEventPublisherService, EventPublisherService>();

            return services;
        }

        private static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ICorrelationService, CorrelationService>();
            services.AddSingleton<IApplicationExceptionHandler, ApplicationExceptionHandler>();
            services.AddTransient(typeof(IEndpointResponse<>), typeof(EndpointResponse<>));
            services.AddTransient<IDatabaseResult, DatabaseResult>();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<IDapperExecutor, DapperExecutor>();
            services.AddSingleton<ISqlServerConnectionFactory, SqlServerConnectionFactory>();
            services.AddLogging();
            services.AddHttpContextAccessor();

            return services;
        }
    }
}
