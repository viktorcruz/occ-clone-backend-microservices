using AutoMapper;
using UsersService.SharedKernel.Mapper;

namespace UsersService.Modules.Mapper
{
    public static class MapperConfiguration
    {
        public static IServiceCollection AddCustomMapper(this IServiceCollection serviceCollection)
        {
            var mapping = new AutoMapper.MapperConfiguration(config =>
            {
                config.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapping.CreateMapper();

            serviceCollection.AddSingleton(mapper);

            return serviceCollection;
        }
    }
}
