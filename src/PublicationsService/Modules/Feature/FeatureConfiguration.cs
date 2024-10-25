namespace PublicationsService.Modules.Feature
{
    public static class FeatureConfiguration
    {
        public static IServiceCollection AddCustomFeature(this IServiceCollection services, IConfiguration configuration)
        {
            string appPolicy = "policy";
            services.AddCors(cnf => cnf.AddPolicy(
                appPolicy, builder => builder.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithOrigins(configuration["Jwt:OriginsCors"])));

            return services;
        }
    }
}
