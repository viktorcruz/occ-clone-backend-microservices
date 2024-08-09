using AutoMapper;
using RolesServices.Aplication.Dto;
using RolesServices.Domain.Entity;

namespace RolesServices.SharedKernel.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RoleEntity, RoleDTO>().ReverseMap();
        }
    }
}

