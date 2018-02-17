using AutoMapper;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Entities;

namespace Inshapardaz.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Dictionary, DictionaryView>()
                .ForMember(s => s.Id, o => o.MapFrom(d => d.Id))
                .ForMember(s => s.Name, o => o.MapFrom(d => d.Name))
                .ForMember(s => s.Language, o => o.MapFrom(d => d.Language))
                .ForMember(s => s.IsPublic, o => o.MapFrom(d => d.IsPublic))
                .ForMember(s => s.UserId, o => o.MapFrom(d => d.UserId))
                .ForMember(s => s.Links, o => o.Ignore())
                .ForMember(s => s.Indexes, o => o.Ignore())
                .ForMember(s => s.WordCount, o => o.Ignore())
                .ReverseMap();

            CreateMap<Word, WordView>()
                .ForMember(s => s.Id, o => o.MapFrom(d => d.Id))
                .ForMember(s => s.Title, o => o.MapFrom(d => d.Title))
                .ForMember(s => s.TitleWithMovements, o => o.MapFrom(d => d.TitleWithMovements))
                .ForMember(s => s.Description, o => o.MapFrom(d => d.Description))
                .ForMember(s => s.Pronunciation, o => o.MapFrom(d => d.Pronunciation))
                .ForMember(s => s.Attributes, o => o.MapFrom(d => d.Attributes))
                .ForMember(s => s.AttributeValue, o => o.MapFrom(d => (int)d.Attributes))
                .ForMember(s => s.Language, o => o.MapFrom(d => d.Language))
                .ForMember(s => s.LanguageId, o => o.MapFrom(d => (int)d.Language))
                .ForMember(s => s.Links, o => o.Ignore())
                .ReverseMap()
                .ForMember(s => s.Relations, o => o.Ignore())
                .ForMember(s => s.Attributes, o => o.MapFrom(d => (GrammaticalType)d.AttributeValue))
                .ForMember(s => s.Language, o => o.MapFrom(d => (Languages)d.LanguageId))
                .ForMember(s => s.Meaning, o => o.Ignore())
                .ForMember(s => s.Translation, o => o.Ignore())
                .ForMember(s => s.DictionaryId, o => o.Ignore());

            CreateMap<Word, SpellingOption>()
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
                .ForMember(d => d.Links, o => o.Ignore());


            CreateMap<Meaning, MeaningView>()
                .ForMember(s => s.Id, o => o.MapFrom(d => d.Id))
                .ForMember(s => s.Context, o => o.MapFrom(d => d.Context))
                .ForMember(s => s.Value, o => o.MapFrom(d => d.Value))
                .ForMember(s => s.Example, o => o.MapFrom(d => d.Example))
                .ForMember(s => s.WordId, o => o.MapFrom(d => d.WordId))
                .ForMember(s => s.Links, o => o.Ignore())
                .ReverseMap();

            CreateMap<Translation, TranslationView>()
                .ForMember(s => s.Id, o => o.MapFrom(d => d.Id))
                .ForMember(s => s.Language, o => o.MapFrom(d => d.Language))
                .ForMember(s => s.LanguageId, o => o.MapFrom(d => (int)d.Language))
                .ForMember(s => s.Value, o => o.MapFrom(d => d.Value))
                .ForMember(s => s.WordId, o => o.MapFrom(d => d.WordId))
                .ForMember(s => s.Links, o => o.Ignore())
                .ForMember(s => s.IsTranspiling, o => o.MapFrom(s => s.IsTrasnpiling))
                .ReverseMap()
                .ForMember(s => s.Language, o => o.MapFrom(d => (Languages)d.LanguageId));

            CreateMap<WordRelation, RelationshipView>()
                .ForMember(s => s.Id, o => o.MapFrom(d => d.Id))
                .ForMember(s => s.RelatedWordId, o => o.MapFrom(d => d.RelatedWordId))
                .ForMember(s => s.RelationType, o => o.MapFrom(d => d.RelationType))
                .ForMember(s => s.RelationTypeId, o => o.MapFrom(d => (int)d.RelationType))
                .ForMember(s => s.RelatedWord, o => o.Ignore())
                .ForMember(s => s.SourceWord, o => o.Ignore())
                .ForMember(s => s.SourceWordId, o => o.Ignore())
                .ForMember(s => s.Links, o => o.Ignore())
                .ReverseMap()
                .ForMember(s => s.RelationType, o => o.MapFrom(d => (RelationType)d.RelationTypeId));
        }
    }
}