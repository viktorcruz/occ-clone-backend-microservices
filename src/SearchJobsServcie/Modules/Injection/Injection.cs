using SharedKernel.Common.Exceptions;
using SharedKernel.Common.Response;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

namespace SearchJobsServcie.Modules.Injection
{
    public static class InjectionConfiguration
    {
        public static IServiceCollection AddCustomInjection(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddSingleton<RabbitMQConnection>();
            //services.AddSingleton<EventBusRabbitMQ>();
            //services.AddSingleton<IEventBus, EventBusRabbitMQ>();

            //services.AddTransient<IEmailService, EmailService>();
            //services.AddTransient<SmtpClient>(sc =>
            //{
            //    return new SmtpClient("smtp.gmail.com")
            //    {
            //        Port = 587,
            //        Credentials = new System.Net.NetworkCredential("@gmail.com", ""),
            //        EnableSsl = true
            //    };
            //});

            //services.AddTransient<EntityOperationEvent>();

            //services.AddSingleton<IEventLogRepository, EventLogRepository>();
            //services.AddSingleton<IDapperExecutor, DapperExecutor>();
            //services.AddTransient<IUserDomain, UserDomain>();
            //services.AddTransient<IUserRepository, UserRepository>();
            //services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            services.AddSingleton<IGlobalExceptionHandler, GlobalExceptionHandler>();
            services.AddTransient(typeof(IEndpointResponse<>), typeof(EndpointResponse<>));
            services.AddTransient<IDatabaseResult, DatabaseResult>();

            services.AddLogging();


            return services;
        }
    }
}
