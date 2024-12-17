using FluentValidation;
using PublicationsService.Application.Commands.Validators;
using PublicationsService.Domain.Core;
using PublicationsService.Domain.Interface;
using PublicationsService.Infrastructure.Interface;
using PublicationsService.Infrastructure.Repository;
using SharedKernel.Common.Repositories;
using SharedKernel.Common.Response;
using SharedKernel.Common.Responses;
using SharedKernel.Data;
using PublicationsService.Application.EventListeners;
using SharedKernel.Common.Messaging;
using SharedKernel.Audit;
using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Common.Interfaces.Persistence;
using SharedKernel.Services;
using SharedKernel.Dapper;
using SharedKernel.Events.Publication;
using SharedKernel.Events.User;
using SharedKernel.Exceptions.Application;
using SharedKernel.Interfaces.Exceptions;
using SharedKernel.Interfaces.Response;
using SharedKernel.Interfaces.Service;
using SharedKernel.Interfaces.Audit;
using SharedKernel.Interfaces.Dapper;
using PublicationsService.Saga;

namespace PublicationsService.Modules.Injection
{
    public static class Injection
    {
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
            services.AddSingleton<RabbitMQSettings>();
            services.AddSingleton<RabbitMQConnection>();
            services.AddSingleton<IAsyncEventBus, RabbitMQEventBus>();
            services.AddSingleton<EventRouter>();

            return services;
        }

        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddTransient<IPublicationDomain, PublicationDomain>();
            services.AddTransient<IAuditEventFactory, AuditEventFactory>();
            services.AddTransient<IEventPublisherService, EventPublisherService>();

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IEventLogStorage, EventLogStorage>();
            services.AddTransient<IPublicationRepository, PublicationRepository>();

            return services;
        }

        public static IServiceCollection AddEventHandler(this IServiceCollection services)
        {
            services.AddScoped<IEventHandler<UserCreatedEvent>, UserCreatedEventHandler>();
            services.AddScoped<IEventHandler<UserCreatedEvent>, UserCreatedEventHandler>();
            services.AddScoped<IEventHandler<UserDeletedEvent>, UserDeletedEventHandler>();
            services.AddScoped<IEventHandler<PublicationCreatedEvent>, PublicationCreatedEventHandler>();

            services.AddScoped<IEventHandler<PublicationCreationFailedEvent>, PublicationCreationFailedEventHandler>();

            return services;
        }

        public static IServiceCollection AddCommonServices(this IServiceCollection services)
        {
            services.AddSingleton<ICorrelationService, CorrelationService>();
            services.AddSingleton<ISqlServerConnectionFactory, SqlServerConnectionFactory>();
            services.AddSingleton<IApplicationExceptionHandler, ApplicationExceptionHandler>();
            services.AddTransient(typeof(IEndpointResponse<>), typeof(EndpointResponse<>));
            services.AddTransient<IDatabaseResult, DatabaseResult>();
            services.AddValidatorsFromAssemblyContaining<CreatePublicationCommandValidator>();
            services.AddSingleton<IDapperExecutor, DapperExecutor>();
            services.AddLogging();
            services.AddHttpContextAccessor();
            services.AddScoped<SagaOrchestrator>();

            return services;
        }
    }
}
