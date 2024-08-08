using AutoMapper;
using UsersService.Application.Dto;
using UsersService.Domain.Entity;

namespace UsersService.SharedKernel.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserEntity, UserRetrieveDTO>().ReverseMap();
        }
    }
}
