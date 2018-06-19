﻿using AutoMapper;
using Inshapardaz.Domain.Entities;

namespace Inshapardaz.Ports.Database
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Dictionary, Entities.Dictionary>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Language, o => o.MapFrom(s => s.Language))
                .ForMember(d => d.IsPublic, o => o.MapFrom(s => s.IsPublic))
                .ForMember(d => d.UserId, o => o.MapFrom(s => s.UserId))
                .ForMember(d => d.Word, o => o.Ignore())
                .ReverseMap()
                .ForMember(d => d.Downloads, o => o.MapFrom(s => s.Downloads));

            CreateMap<DictionaryDownload, Entities.DictionaryDownload>()
                .ForMember(d => d.DictionaryId, o => o.MapFrom(s => s.DictionaryId))
                .ForMember(d => d.MimeType, o => o.MapFrom(s => s.MimeType))
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Dictionary, o => o.Ignore())
                .ForMember(d => d.File, o => o.MapFrom(s => s.File))
                .ForMember(d => d.FileId, o => o.Ignore())
                .ReverseMap();


            CreateMap<Word, Entities.Word>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Attributes, o => o.MapFrom(s => s.Attributes))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
                .ForMember(d => d.Dictionary, o => o.Ignore())
                .ForMember(d => d.DictionaryId, o => o.MapFrom(s => s.DictionaryId))
                .ForMember(d => d.Language, o => o.MapFrom(s => s.Language))
                .ForMember(d => d.Meaning, o => o.MapFrom(s => s.Meaning))
                .ForMember(d => d.Pronunciation, o => o.MapFrom(s => s.Pronunciation))
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
                .ForMember(d => d.TitleWithMovements, o => o.MapFrom(s => s.TitleWithMovements))
                .ForMember(d => d.Translation, o => o.MapFrom(s => s.Translation))
                .ForMember(d => d.WordRelationRelatedWord, o => o.MapFrom(s => s.WordRelationRelatedWord))
                .ForMember(d => d.WordRelationSourceWord, o => o.MapFrom(s => s.WordRelationSourceWord))
                .ReverseMap()
                .ForMember(d => d.Relations, o => o.Ignore());

            CreateMap<Meaning, Entities.Meaning>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Context, o => o.MapFrom(s => s.Context))
                .ForMember(d => d.Example, o => o.MapFrom(s => s.Example))
                .ForMember(d => d.Value, o => o.MapFrom(s => s.Value))
                .ForMember(d => d.Word, o => o.Ignore())
                .ForMember(d => d.WordId, o => o.MapFrom(s => s.WordId))
                .ReverseMap()
                .ForMember(d => d.Word, o => o.Ignore());

            CreateMap<Translation, Entities.Translation>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.IsTrasnpiling, o => o.MapFrom(s => s.IsTrasnpiling))
                .ForMember(d => d.Language, o => o.MapFrom(s => s.Language))
                .ForMember(d => d.Value, o => o.MapFrom(s => s.Value))
                .ForMember(d => d.Word, o => o.Ignore())
                .ForMember(d => d.WordId, o => o.MapFrom(s => s.WordId))
                .ReverseMap()
                .ForMember(d => d.Word, o => o.Ignore());

            CreateMap<WordRelation, Entities.WordRelation>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.RelatedWord, o => o.Ignore())
                .ForMember(d => d.RelatedWordId, o => o.MapFrom(s => s.RelatedWordId))
                .ForMember(d => d.RelationType, o => o.MapFrom(s => s.RelationType))
                .ForMember(d => d.SourceWord, o => o.Ignore())
                .ForMember(d => d.SourceWordId, o => o.MapFrom(s => s.SourceWordId))
                .ReverseMap()
                .ForMember(d => d.RelatedWord, o => o.MapFrom(s => s.RelatedWord))
                .ForMember(d => d.SourceWord, o => o.MapFrom(s => s.SourceWord));
        }
    }
}