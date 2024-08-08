using UsersService.Domain.Core;
using UsersService.Domain.Interface;
using UsersService.Infrastructure.Interface;
using UsersService.Infrastructure.Repository;
using UsersService.Persistence.Data;
using UsersService.Persistence.Interface;
using UsersService.SharedKernel.Common.Response;
using UsersService.SharedKernel.Interface;

namespace UsersService.Modules.Injection
{
    public static class CustomInjectionExtensions
    {
        public static IServiceCollection AddCustomInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IUsersDomain, UsersDomain>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            services.AddSingleton<IExceptionManagement, ExceptionManagement>();
            services.AddSingleton<ISpResult, SpResult>();

            return services;
        }
    }
}
