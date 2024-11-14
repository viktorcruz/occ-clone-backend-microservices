using AuthService.Application.Interfaces;
using AuthService.Application.UsesCases;
using AuthService.Domain.Core;
using AuthService.Domain.Events;
using AuthService.Domain.Interfaces;
using AuthService.Domain.Ports.Output;
using AuthService.Domain.Ports.Output.Services;
using AuthService.Infrastructure.Adapters;
using AuthService.Infrastructure.Messaging;
using AuthService.Infrastructure.Repository;
using AuthService.Infrastructure.Services;
using SharedKernel.Common;
using SharedKernel.Common.Exceptions;
using SharedKernel.Common.Interfaces;
using SharedKernel.Common.Response;
using SharedKernel.Common.Responses;
using SharedKernel.Data;
using SharedKernel.Interface;

namespace AuthService.Modules.Injection
{
    public static class Injection
    {
        //public static IServiceCollection AddCustomInjections(this IServiceCollection services)
        //{

            //services.AddScoped<RabbitMQConnection>();
            //services.AddScoped<EventBusRabbitMQ>();
            //services.AddScoped<IEventBus, EventBusRabbitMQ>();
            //services.AddScoped<IEventPublisherService, EventPublisherService>();

            //services.AddScoped<IEventLogPort, EventLogRepository>();

            //services.AddScoped<EntityOperationEvent>();
            //services.AddScoped<IEntityOperationEventFactory, EntityOperationEventFactory>();
            //services.AddScoped<IDapperExecutor, DapperExecutor>();

            //services.AddScoped<ILoginUseCase, LoginUseCase>();
            //services.AddScoped<IRegisterUseCase, RegisterUseCase>();
            //services.AddScoped<IRenewTokenUseCase, RenewTokenUseCase>();
            //services.AddScoped<IConfirRegisterUserCase, ConfirmRegsiterUseCase>();

            //services.AddScoped<ILoginPort, LoginAdapter>();
            //services.AddScoped<IRegisterPort, RegisterAdapter>();
            //services.AddScoped<IRenewTokenPort, RenewTokenAdapter>();
            //services.AddScoped<IConfirmRegisterPort, ConfirmRegisterAdapter>();

            //services.AddScoped<IUserPort, UserRepository>();
            //services.AddScoped<IRolePort, RoleRepository>();
            //services.AddScoped<IRegisterUserPort, RegisterRepository>();
            //services.AddScoped<ITokenService, TokenService>();

            //services.AddScoped<ISqlServerConnectionFactory>();
            //services.AddScoped<IGlobalExceptionHandler, GlobalExceptionHandler>();
            //services.AddScoped(typeof(IEndpointResponse<>), typeof(EndpointResponse<>));
            //services.AddScoped<IDatabaseResult, DatabaseResult>();

        //    return services;
        //}

        public static IServiceCollection AddCustomInjection(this IServiceCollection services)
        {
            services.AddRabbitMQ();
            services.AddApplications();
            services.AddDomainServices();
            services.AddRepositories();
            services.AddAdapters();
            services.AddCommonServices();

            return services;
        }

        public static IServiceCollection AddRabbitMQ(this IServiceCollection services)
        {
            services.AddScoped<RabbitMQConnection>();
            services.AddScoped<RabbitMQEventBus>();
            services.AddScoped<IEventBus, RabbitMQEventBus>();
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
            services.AddScoped<EntityOperationEvent>();
            services.AddScoped<IEntityOperationEventFactory, EntityOperationEventFactory>();           

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IEventLogPort, EventLogRepository>();
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

            return services;
        }
    }

}
