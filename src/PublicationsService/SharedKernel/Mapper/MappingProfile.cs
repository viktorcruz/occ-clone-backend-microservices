using AutoMapper;
using PublicationsService.Aplication.DTO;
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
