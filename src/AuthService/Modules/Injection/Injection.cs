using AuthService.Application.Interfaces;
using AuthService.Application.UsesCases;
using AuthService.Domain.Ports.Output;
using AuthService.Domain.Ports.Output.Services;
using AuthService.Infrastructure.Adapters;
using AuthService.Infrastructure.Repository;
using AuthService.Infrastructure.Services;
using SharedKernel.Audit;
using SharedKernel.Common.Interfaces.EventBus;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Common.Interfaces.Persistence;
using SharedKernel.Common.Messaging;
using SharedKernel.Common.Repositories;
using SharedKernel.Common.Response;
using SharedKernel.Common.Responses;
using SharedKernel.Dapper;
using SharedKernel.Data;
using SharedKernel.Exceptions.Application;
using SharedKernel.Interfaces.Audit;
using SharedKernel.Interfaces.Dapper;
using SharedKernel.Interfaces.Exceptions;
using SharedKernel.Interfaces.Response;
using SharedKernel.Interfaces.Service;
using SharedKernel.Services;

namespace AuthService.Modules.Injection
{
    public static class Injection
    {
        public static IServiceCollection AddCustomInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRabbitMQ(configuration);
            services.AddApplications();
            services.AddDomainServices();
            services.AddRepositories();
            services.AddAdapters();
            services.AddCommonServices();

            return services;
        }

        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<RabbitMQSettings>();
            services.AddScoped<RabbitMQConnection>();
            services.AddScoped<IAsyncEventBus, RabbitMQEventBus>();
            services.AddScoped<IEventPublisherService, EventPublisherService>();

            return services;
        }

        public static IServiceCollection AddApplications(this IServiceCollection services)
        {
            services.AddScoped<ILoginUseCase, LoginUseCase>();
            services.AddScoped<IRegisterUseCase, RegisterUseCase>();
            services.AddScoped<IRenewTokenUseCase, RenewTokenUseCase>();
            services.AddScoped<IConfirRegisterUserCase, ConfirmRegsiterUseCase>();

            return services;
        }

        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IAuditEventFactory, AuditEventFactory>();           

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IEventLogStorage, EventLogStorage>();
            services.AddScoped<IUserPort, UserRepository>();
            services.AddScoped<IRolePort, RoleRepository>();
            services.AddScoped<IRegisterUserPort, RegisterRepository>();

            return services;
        }

        public static IServiceCollection AddAdapters(this IServiceCollection services)
        {
            services.AddScoped<ILoginPort, LoginAdapter>();
            services.AddScoped<IRegisterPort, RegisterAdapter>();
            services.AddScoped<IRenewTokenPort, RenewTokenAdapter>();
            services.AddScoped<IConfirmRegisterPort, ConfirmRegisterAdapter>();

            return services;
        }

        public static IServiceCollection AddCommonServices(this IServiceCollection services)
        {
            services.AddScoped<ISqlServerConnectionFactory, SqlServerConnectionFactory>();
            services.AddScoped<IApplicationExceptionHandler, ApplicationExceptionHandler>();
            services.AddScoped(typeof(IEndpointResponse<>), typeof(EndpointResponse<>));
            services.AddScoped<IDatabaseResult, DatabaseResult>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IDapperExecutor, DapperExecutor>();
            services.AddLogging();
            services.AddScoped<ICorrelationService, CorrelationService>();
            services.AddHttpContextAccessor();

            return services;
        }
    }

}
