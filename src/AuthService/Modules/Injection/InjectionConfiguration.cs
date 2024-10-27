using AuthService.Application.Interfaces;
using AuthService.Application.UsesCases;
using AuthService.Domain.Ports.Output.Repositories;
using AuthService.Domain.Ports.Output.Services;
using AuthService.Factories;
using AuthService.Factories.Interface;
using AuthService.Infrastructure.Adapters;
using AuthService.Infrastructure.Repositories;
using AuthService.Infrastructure.Services;
using SharedKernel.Common.Exceptions;
using SharedKernel.Interface;

namespace AuthService.Modules.Injection
{
    public static class InjectionConfiguration
    {
        public static IServiceCollection AddCustomInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

            services.AddScoped<ILoginUseCase, LoginUseCase>();
            services.AddScoped<IRegisterUseCase, RegisterUseCase>();
            services.AddScoped<IRenewTokenUseCase, RenewTokenUseCase>();

            services.AddScoped<ILoginPort, LoginAdapter>();
            services.AddScoped<IRegisterPort, RegisterAdapter>();
            services.AddScoped<IRenewTokenPort, RenewTokenAdapter>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRegisterRepository, RegisterRepository>();
            services.AddScoped<IGlobalExceptionHandler, GlobalExceptionHandler>();
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }

}
