namespace UsersService.Modules.Feature
{
    public static class FeatureConfiguration
    {
        public static IServiceCollection AddCustomFeature(this IServiceCollection services, IConfiguration configuration)
        {
            string appPolicy = "policy";

            services.AddCors(cfg => cfg.AddPolicy(
                appPolicy, builder => builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins(configuration["Config:OriginsCors"])));

            return services;
        }
    }
}
