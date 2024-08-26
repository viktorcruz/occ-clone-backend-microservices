using SharedKernel.Common;
using SharedKernel.Common.Exceptions;
using SharedKernel.Common.Response;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using UsersService.Domain.Core;
using UsersService.Domain.Interface;
using UsersService.Infrastructure.Interface;
using UsersService.Infrastructure.Messaging;
using UsersService.Infrastructure.Repository;
using UsersService.Persistence.Data;
using UsersService.Persistence.Interface;

namespace UsersService.Modules.Injection
{
    public static class InjectionConfiguration
    {
        public static IServiceCollection AddCustomInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<RabbitMQConnection>();
            services.AddSingleton<EventBusRabbitMQ>();

            services.AddSingleton<IEventLogRepository, EventLogRepository>();
            services.AddSingleton<IDapperExecutor, DapperExecutor>();

            services.AddTransient<IUserDomain, UserDomain>();
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            services.AddSingleton<IGlobalExceptionHandler, GlobalExceptionHandler>();
            services.AddTransient(typeof(IEndpointResponse<>), typeof(EndpointResponse<>));
            services.AddTransient<IDatabaseResult, DatabaseResult>();

            return services;
        }
    }
}
