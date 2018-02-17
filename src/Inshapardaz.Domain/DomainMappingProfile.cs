using AutoMapper;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain
{
    public class DomainMappingProfile : Profile
    {
        public DomainMappingProfile()
        {
            CreateMap<Dictionary, DictionaryModel>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Language, o => o.MapFrom(s => s.Language))
                .ForMember(d => d.IsPublic, o => o.MapFrom(s => s.IsPublic))
                .ForMember(d => d.UserId, o => o.MapFrom(s => s.UserId))
                .ReverseMap()
                .ForMember(d => d.Downloads, o => o.Ignore());
        }
    }
}