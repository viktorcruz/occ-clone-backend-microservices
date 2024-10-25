using AutoMapper;
using PublicationsService.Aplication.Dto;
using PublicationsService.Domain.Entity;

namespace PublicationsService.SharedKernel.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<PublicationEntity, PublicationDTO>().ReverseMap();
        }
    }
}
