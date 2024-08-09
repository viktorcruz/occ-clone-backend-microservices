using RolesServices.Domain.Core;
using RolesServices.Domain.Interface;
using RolesServices.Infrastructure.Interface;
using RolesServices.Infrastructure.Repository;
using RolesServices.Persistence.Data;
using RolesServices.Persistence.Interface;
using SharedKernel.Common.Exceptions;
using SharedKernel.Common.Response;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

namespace RolesServices.Modules.Injection
{
    public static class InjectionConfiguration
    {
        public static IServiceCollection AddCustomInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IRoleDomain, RoleDomain>();
            services.AddTransient<IRoleRespository, RoleRepository>();

            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            services.AddSingleton<IGlobalExceptionHandler, GlobalExceptionHandler>();
            services.AddTransient(typeof(IEndpointResponse<>), typeof(EndpointResponse<>));
            services.AddTransient<IDatabaseResult, DatabaseResult>();

            return services;
        }
    }
}
