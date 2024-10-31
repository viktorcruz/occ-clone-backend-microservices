namespace SearchJobsServcie.Modules.Feature
{
    public static class Feature
    {
        public static IServiceCollection AddCustomFeature(this IServiceCollection services, IConfiguration configuration)
        {
            string appPolicy = "policy";
            services.AddCors(
                cnf => cnf.AddPolicy(
                    appPolicy,
                    builder => builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins(configuration["Jwt:OriginCors"])));

            return services;
        }
    }
}
