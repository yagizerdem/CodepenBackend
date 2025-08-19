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

            CreateMap<CreatePenDTO, PenEntity>()
                .ForMember(dest => dest.Author, opt => opt.Ignore())
                .ForMember(dest => dest.OldVersions, opt => opt.Ignore());
            
            CreateMap<UpdatePenDTO, PenEntity>()
               .ForAllMembers(opt =>
                   opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<PenEntity, OldPenVersionsEntity>().
                ForMember(dest => dest.Id, opt =>  opt.Ignore()); // identity insert is off by default in .net

        }
    }
}
