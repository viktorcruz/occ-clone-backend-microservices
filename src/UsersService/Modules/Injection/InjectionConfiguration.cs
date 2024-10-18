using SharedKernel.Common;
using SharedKernel.Common.Exceptions;
using SharedKernel.Common.Response;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using System.Net.Mail;
using UsersService.Domain.Core;
using UsersService.Domain.Events;
using UsersService.Domain.Interface;
using UsersService.Infrastructure.Interface;
using UsersService.Infrastructure.Messaging;
using UsersService.Infrastructure.Repository;
using UsersService.Persistence.Data;
using UsersService.Persistence.Interface;
using UsersService.SharedKernel.Service;
using UsersService.SharedKernel.Service.Interface;

namespace UsersService.Modules.Injection
{
    public static class InjectionConfiguration
    {
        public static IServiceCollection AddCustomInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<RabbitMQConnection>();
            services.AddSingleton<EventBusRabbitMQ>();
            services.AddSingleton<IEventBus, EventBusRabbitMQ>();

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

            services.AddTransient<EntityOperationEvent>();

            services.AddSingleton<IEventLogRepository, EventLogRepository>();
            services.AddSingleton<IDapperExecutor, DapperExecutor>();
            services.AddTransient<IUserDomain, UserDomain>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            services.AddSingleton<IGlobalExceptionHandler, GlobalExceptionHandler>();
            services.AddTransient(typeof(IEndpointResponse<>), typeof(EndpointResponse<>));
            services.AddTransient<IDatabaseResult, DatabaseResult>();

            services.AddLogging();


            return services;
        }
    }
}
