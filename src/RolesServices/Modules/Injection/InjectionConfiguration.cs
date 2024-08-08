namespace RolesServices.Modules.Injection
{
    public static class InjectionConfiguration
    {
        public static IServiceCollection AddCustomInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IUsersDomain, UsersDomain>();
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            services.AddSingleton<IGlobalExceptionHandler, GlobalExceptionHandler>();
            services.AddTransient(typeof(IEndpointResponse<>), typeof(EndpointResponse<>));
            services.AddTransient<IDatabaseResult, DatabaseResult>();

            return services;
        }
    }
}
