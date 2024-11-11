using SharedKernel.Common;
using SharedKernel.Common.Exceptions;
using SharedKernel.Common.Interfaces;
using SharedKernel.Common.Messaging;
using SharedKernel.Common.Response;
using SharedKernel.Common.Responses;
using SharedKernel.Data;
using SharedKernel.Interface;
using System.Net.Mail;
using UsersService.Domain.Core;
using UsersService.Domain.Events;
using UsersService.Domain.Interface;
using UsersService.Domain.Servcies;
using UsersService.Events;
using UsersService.Events.Handlers;
using UsersService.Infrastructure.Interface;
using UsersService.Infrastructure.Repository;
using UsersService.SharedKernel.Service;
using UsersService.SharedKernel.Service.Interface;

namespace UsersService.Modules.Injection
{
    public static class Injection
    {
        //public static IServiceCollection AddCustomInjectionx(this IServiceCollection services, IConfiguration configuration)
        //{
        //    services.AddSingleton<RabbitMQConnection>();
        //    services.AddSingleton<EventBusRabbitMQ>();
        //    services.AddSingleton<IEventBus, EventBusRabbitMQ>();

        //    services.AddTransient<IEventHandler<UserCreatedEvent>, UserCreatedEventHandler>();
        //    //services.AddTransient<IEventHandler<UserCreationSucceededEvent>, UserSagaHandler>();
        //    //services.AddTransient<IEventHandler<UserCreationFailedEvent>, UserSagaHandler>();

        //    services.AddTransient<IEmailService, EmailService>();
        //    services.AddTransient<SmtpClient>(sc =>
        //    {
        //        return new SmtpClient("smtp.gmail.com")
        //        {
        //            Port = 587,
        //            Credentials = new System.Net.NetworkCredential("@gmail.com", ""),
        //            EnableSsl = true
        //        };
        //    });

        //    services.AddTransient<EntityOperationEvent>();
        //    services.AddTransient<IEntityOperationEventFactory, EntityOperationEventFactory>();

        //    services.AddSingleton<IEventLogRepository, EventLogRepository>();
        //    services.AddSingleton<IDapperExecutor, DapperExecutor>();
        //    services.AddTransient<IUserDomain, UserDomain>();
        //    services.AddTransient<IUserRepository, UserRepository>();
        //    //services.AddTransient<ISearchJobsRepository, SearchJobsRepository>();
        //    services.AddSingleton<ISqlServerConnectionFactory, SqlServerConnectionFactory>();
        //    services.AddSingleton<IGlobalExceptionHandler, GlobalExceptionHandler>();
        //    services.AddTransient(typeof(IEndpointResponse<>), typeof(EndpointResponse<>));
        //    services.AddTransient<IDatabaseResult, DatabaseResult>();

        //    services.AddLogging();


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
            services.AddSingleton<RabbitMQEventBus>();
            services.AddSingleton<IEventBus, RabbitMQEventBus>();

            return services;
        }

        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddTransient<IUserDomain, UserDomain>();
            services.AddTransient<IEntityOperationEventFactory, EntityOperationEventFactory>();
            services.AddTransient<EntityOperationEvent>();

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddSingleton<IEventLogRepository, EventLogRepository>();

            return services;
        }

        public static IServiceCollection AddEventHandler(this IServiceCollection services)
        {
            services.AddTransient<IEventHandler<UserCreatedEvent>, UserCreatedEventHandler>();
            services.AddScoped<IEventPublisherService, EventPublisherService>();
            //services.AddTransient<IEventHandler<UserCreationSucceededEvent>, UserSagaHandler>();
            //services.AddTransient<IEventHandler<UserCreationFailedEvent>, UserSagaHandler>();

            return services;
        }

        public static IServiceCollection AddCommonServices(this IServiceCollection services)
        {
            services.AddSingleton<ISqlServerConnectionFactory, SqlServerConnectionFactory>();
            services.AddSingleton<IGlobalExceptionHandler, GlobalExceptionHandler>();
            services.AddTransient(typeof(IEndpointResponse<>), typeof(EndpointResponse<>));
            services.AddTransient<IDatabaseResult, DatabaseResult>();
            services.AddSingleton<IDapperExecutor, DapperExecutor>();
            services.AddLogging();

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
    }
}
