using FluentValidation;
using PublicationsService.Application.Commands.Validators;
using PublicationsService.Domain.Core;
using PublicationsService.Domain.Events;
using PublicationsService.Domain.Interface;
using PublicationsService.Infrastructure.Interface;
using PublicationsService.Infrastructure.Repository;
using SharedKernel.Common;
using SharedKernel.Common.Exceptions;
using SharedKernel.Common.Interfaces;
using SharedKernel.Common.Repositories;
using SharedKernel.Common.Response;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using SharedKernel.Data;
using SharedKernel.Common.Events;
using PublicationsService.Application.EventListeners;
using SharedKernel.Common.Messaging;
using PublicationsService.Domain.Services;

namespace PublicationsService.Modules.Injection
{
    public static class Injection
    {
        //public static IServiceCollection AddCustomInjections(this IServiceCollection services, IConfiguration configuration)
        //{
        //    //services.AddSingleton<RabbitMQConnection>();
        //    //services.AddSingleton<RabbitMQEventBus>();
        //    //services.AddSingleton<IEventBus, RabbitMQEventBus>();


        //    services.AddTransient<EntityOperationEvent>();
        //    services.AddTransient<IEntityOperationEventFactory, EntityOperationEventFactory>();
        //    services.AddSingleton<IDapperExecutor, DapperExecutor>();
        //    services.AddSingleton<IEventLogRepository, EventLogRepository>();
        //    services.AddTransient<IPublicationDomain, PublicationDomain>();
        //    services.AddTransient<IPublicationRepository, PublicationRepository>();
        //    //services.AddTransient<ISearchJobsRepository, SearchJobsRepository>();
        //    services.AddSingleton<ISqlServerConnectionFactory, SqlServerConnectionFactory>();
        //    services.AddSingleton<IGlobalExceptionHandler, GlobalExceptionHandler>();
        //    services.AddTransient(typeof(IEndpointResponse<>), typeof(EndpointResponse<>));
        //    services.AddTransient<IDatabaseResult, DatabaseResult>();

        //    services.AddValidatorsFromAssemblyContaining<CreatePublicationCommandValidator>();



        //    // Registro de handlers específicos de PublicationsService
        //    services.AddScoped<IEventHandler<UserCreatedEvent>, UserCreatedEventHandler>();

        //    var serviceProvider = services.BuildServiceProvider();
        //    var eventBus = serviceProvider.GetRequiredService<IEventBus>();

        //    // Crear y configurar el EventRouter para este microservicio
        //    var eventRouter = new EventRouter(serviceProvider, eventBus);

        //    // Registrar solo los eventos que necesita PublicationsService
        //    // Registrar eventos en el EventRouter
        //    eventRouter.RegisterEventHandler<UserCreatedEvent>("user_exchange", "user.created");
        //    eventRouter.RegisterEventHandler<UserDeletedEvent>("user_exchange", "user.deleted");


        //    return services;
        //}

        public static IServiceCollection AddCustomInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRabbitMQ(configuration);
            services.AddDomainServices();
            services.AddRepositories();
            services.AddEventHandler();
            services.AddCommonServices();

            return services;
        }

        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<RabbitMQConnection>();
            services.AddSingleton<IEventBus, RabbitMQEventBus>();
            services.AddSingleton<EventRouter>();

            return services;
        }

        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddTransient<IPublicationDomain, PublicationDomain>();
            services.AddTransient<IEntityOperationEventFactory, EntityOperationEventFactory>();
            services.AddTransient<IEventPublisherService, EventPublisherService>();

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IEventLogRepository, EventLogRepository>();
            services.AddTransient<IPublicationRepository, PublicationRepository>();

            return services;
        }

        public static IServiceCollection AddEventHandler(this IServiceCollection services)
        {
            services.AddScoped<IEventHandler<UserCreatedEvent>, UserCreatedEventHandler>();

            return services;
        }

        public static IServiceCollection AddCommonServices(this IServiceCollection services)
        {
            services.AddSingleton<ISqlServerConnectionFactory, SqlServerConnectionFactory>();
            services.AddSingleton<IGlobalExceptionHandler, GlobalExceptionHandler>();
            services.AddTransient(typeof(IEndpointResponse<>), typeof(EndpointResponse<>));
            services.AddTransient<IDatabaseResult, DatabaseResult>();
            services.AddValidatorsFromAssemblyContaining<CreatePublicationCommandValidator>();
            services.AddSingleton<IDapperExecutor, DapperExecutor>();
            services.AddLogging();

            return services;
        }
    }
}
