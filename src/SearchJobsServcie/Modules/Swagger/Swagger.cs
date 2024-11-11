using Microsoft.OpenApi.Models;
using System.Reflection;

namespace SearchJobsService.Modules.Swagger
{
    public static class Swagger
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(cnf =>
            {
                cnf.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "OCC", });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    cnf.IncludeXmlComments(xmlPath);
                }

                cnf.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description = "Authorization by API KEY",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Name = "Authorization"
                    }
                );

                cnf.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header
                            },
                            new List<string>()
                        }
                    }
                );
            });

            return services;
        }
    }
}
