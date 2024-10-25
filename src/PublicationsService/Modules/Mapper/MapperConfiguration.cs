using AutoMapper;
using PublicationsService.SharedKernel.Mapper;

namespace PublicationsService.Modules.Mapper
{
    public static class MapperConfiguration
    {
        public static IServiceCollection AddCustomMapper(this IServiceCollection services, IConfiguration configuration)
        {
            var mapping = new AutoMapper.MapperConfiguration(cnf =>
            {
                cnf.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapping.CreateMapper();
            services.AddSingleton(mapper);

            return services;
        }
    }
}
