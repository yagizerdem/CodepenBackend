using Models.DTO;
using Models.Entity;
using AutoMapper;

namespace CodePen
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterDTO, ApplicationUserEntity>();
            
        }
    }
}
