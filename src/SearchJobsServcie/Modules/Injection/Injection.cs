using SearchJobsService.Domain.Core;
using SearchJobsService.Domain.Events;
using SearchJobsService.Domain.Interface;
using SearchJobsService.Infrastructure.Interface;
using SearchJobsService.Infrastructure.Repository;
using SearchJobsService.Application.EventListeners;
using SharedKernel.Common;
using SharedKernel.Common.Events;
using SharedKernel.Common.Exceptions;
using SharedKernel.Common.Interfaces;
using SharedKernel.Common.Messaging;
using SharedKernel.Common.Repositories;
using SharedKernel.Common.Response;
using SharedKernel.Common.Responses;
using SharedKernel.Data;
using SharedKernel.Interface;
using SearchJobsService.Domain.Services;

namespace SearchJobsService.Modules.Injection
{
    public static class Injection
    {
        //public static IServiceCollection AddCustomInjections(this IServiceCollection services, IConfiguration configuration)
        //{
        //    services.AddSingleton<RabbitMQConnection>();
        //    services.AddSingleton<RabbitMQEventBus>();
        //    services.AddTransient<IEventBus, RabbitMQEventBus>();


        //    services.AddTransient<EntityOperationEvent>();
        //    services.AddTransient<IEntityOperationEventFactory, EntityOperationEventFactory>();
        //services.AddSingleton<IDapperExecutor, DapperExecutor>();
        //    services.AddTransient<IEventLogRepository, EventLogRepository>();
        //    services.AddTransient<ISearchJobsDomain, SearchJobsDomain>();
        //    services.AddTransient<ISearchJobsRepository, SearchJobsRepository>();
        //    services.AddSingleton<IGlobalExceptionHandler, GlobalExceptionHandler>();
        //    services.AddTransient(typeof(IEndpointResponse<>), typeof(EndpointResponse<>));
        //    services.AddTransient<IDatabaseResult, DatabaseResult>();
        //    services.AddSingleton<ISqlServerConnectionFactory, SqlServerConnectionFactory>();

        //    services.AddLogging();


        //    // Registra los handlers de eventos
        //    services.AddScoped<IEventHandler<UserCreatedEvent>, UserCreatedEventHandler>();
        //    services.AddScoped<IEventHandler<UserCreationSucceededEvent>, UserCreationSucceededEventHandler>();

        //    // Suscribe a los eventos en RabbitMQ
        //    var serviceProvider = services.BuildServiceProvider();
        //    var eventBus = serviceProvider.GetRequiredService<IEventBus>();


        //    // Crear y configurar el EventRouter para este microservicio
        //    var eventRouter = new EventRouter(serviceProvider, eventBus);

        //    // Registrar solo los eventos que necesita SearchJobsService
        //    // Registrar eventos en el EventRouter
        //    eventRouter.RegisterEventHandler<UserCreatedEvent>("user_exchange", "user.created");
        //    eventRouter.RegisterEventHandler<UserUpdatedEvent>("user_exchange", "user.updated");
        //    eventRouter.RegisterEventHandler<PublicationCreatedEvent>("publication_exchange", "publication.created");


        //    return services;
        //}

        public static IServiceCollection AddCustomInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRabbitMQ(configuration);
            services.AddRepositories();
            services.AddDomainServices();
            services.AddEventHandler();
            services.AddCommonServices(configuration);

            return services;
        }

        private static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<RabbitMQConnection>();
            services.AddSingleton<IEventBus, RabbitMQEventBus>();
            services.AddSingleton<EventRouter>();

            return services;
        }

        private static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddTransient<ISearchJobsDomain, SearchJobsDomain>();
            services.AddTransient<IEntityOperationEventFactory, EntityOperationEventFactory>();

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IEventLogRepository, EventLogRepository>();
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
            services.AddSingleton<IGlobalExceptionHandler, GlobalExceptionHandler>();
            services.AddTransient(typeof(IEndpointResponse<>), typeof(EndpointResponse<>));
            services.AddTransient<IDatabaseResult, DatabaseResult>();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<IDapperExecutor, DapperExecutor>();
            services.AddSingleton<ISqlServerConnectionFactory, SqlServerConnectionFactory>();
            services.AddLogging();

            return services;
        }
    }
}
