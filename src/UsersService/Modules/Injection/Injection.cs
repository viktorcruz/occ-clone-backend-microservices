using SharedKernel.Audit;
using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Common.Interfaces.Persistence;
using SharedKernel.Common.Messaging;
using SharedKernel.Common.Response;
using SharedKernel.Common.Responses;
using SharedKernel.Dapper;
using SharedKernel.Data;
using SharedKernel.Events.Auth;
using SharedKernel.Events.JobSearch;
using SharedKernel.Events.Publication;
using SharedKernel.Events.User;
using SharedKernel.Exceptions.Application;
using SharedKernel.Interfaces.Audit;
using SharedKernel.Interfaces.Dapper;
using SharedKernel.Interfaces.Exceptions;
using SharedKernel.Interfaces.Response;
using SharedKernel.Interfaces.Service;
using SharedKernel.Services;
using System.Net.Mail;
using UsersService.Application.EventListeners;
using UsersService.Domain.Core;
using UsersService.Domain.Interface;
using UsersService.Events.Handlers;
using UsersService.Infrastructure.Interface;
using UsersService.Infrastructure.Repository;
using UsersService.Saga;
using UsersService.Saga.Interfaces;
using UsersService.SharedKernel.Service;
using UsersService.SharedKernel.Service.Interface;

namespace UsersService.Modules.Injection
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
            services.AddSaga();

            return services;
        }

        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<RabbitMQSettings>();
            services.AddSingleton<RabbitMQConnection>();
            services.AddSingleton<RabbitMQEventBus>();
            services.AddSingleton<IAsyncEventBus, RabbitMQEventBus>();

            return services;
        }

        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddTransient<IUserDomain, UserDomain>();
            services.AddTransient<IAuditEventFactory, AuditEventFactory>();
            //services.AddTransient<EntityOperationEvent>();

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddSingleton<IEventLogStorage, EventLogStore>();

            return services;
        }

        public static IServiceCollection AddEventHandler(this IServiceCollection services)
        {
            services.AddScoped<IEventPublisherService, EventPublisherService>();
            services.AddSingleton<EventRouter>();
            services.AddScoped<IEventHandler<RegisterSuccessEvent>, UserSagaHandler>();
            services.AddScoped<IEventHandler<RegisterErrorEvent>, UserSagaHandler>();
            services.AddScoped<CompensationActions>();
            services.AddScoped<UserSagaContext>();


            services.AddTransient<IEventHandler<UserCreatedEvent>, UserCreatedEventHandler>();
            services.AddScoped<IEventHandler<JobSearchApplyEvent>, SearchJobsApplyEventHandler>();
            services.AddScoped<IEventHandler<UserUpdatedEvent>, UserUpdatedEventHandler>();
            services.AddScoped<IEventHandler<PublicationCreatedEvent>, PublicationCreatedEventHandler>();

            return services;
        }

        public static IServiceCollection AddCommonServices(this IServiceCollection services)
        {
            services.AddSingleton<ICorrelationService, CorrelationService>();
            services.AddSingleton<ISqlServerConnectionFactory, SqlServerConnectionFactory>();
            services.AddSingleton<IApplicationExceptionHandler, ApplicationExceptionHandler>();
            services.AddTransient(typeof(IEndpointResponse<>), typeof(EndpointResponse<>));
            services.AddTransient<IDatabaseResult, DatabaseResult>();
            services.AddSingleton<IDapperExecutor, DapperExecutor>();
            services.AddLogging();
            services.AddHttpContextAccessor();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<SmtpClient>(sc =>
            {
                return new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new System.Net.NetworkCredential("@gmail.com", ""),
                    EnableSsl = true
                };
            });

            return services;
        }

        public static IServiceCollection AddSaga(this IServiceCollection services)
        {
            services.AddScoped<IUserSagaContext, UserSagaContext>();
            services.AddScoped<IUserSagaHandler, UserSagaHandler>();
            services.AddScoped<ICompensationActions, CompensationActions>();            

            return services;
        }
    }
}
