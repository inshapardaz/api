using AutoMapper;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Entities.Library;

namespace Inshapardaz.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<File, FileView>()
                .ForMember(s => s.Id, o => o.MapFrom(d => d.Id))
                .ForMember(s => s.DateCreated, o => o.MapFrom(d => d.DateCreated))
                .ForMember(s => s.FileName, o => o.MapFrom(d => d.FileName))
                .ForMember(s => s.MimeType, o => o.MapFrom(d => d.MimeType))
                .ForMember(d => d.Links, o => o.Ignore())
                .ReverseMap();

            CreateMap<Dictionary, DictionaryView>()
                .ForMember(s => s.Id, o => o.MapFrom(d => d.Id))
                .ForMember(s => s.Name, o => o.MapFrom(d => d.Name))
                .ForMember(s => s.Language, o => o.MapFrom(d => d.Language))
                .ForMember(s => s.IsPublic, o => o.MapFrom(d => d.IsPublic))
                .ForMember(s => s.UserId, o => o.MapFrom(d => d.UserId))
                .ForMember(d => d.Links, o => o.Ignore())
                .ForMember(d => d.Indexes, o => o.Ignore())
                .ForMember(d => d.WordCount, o => o.MapFrom(s => s.WordCount))
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
                .ForMember(s => s.RelatedWord, o => o.MapFrom(d => d.RelatedWord.Title))
                .ForMember(s => s.SourceWord, o => o.MapFrom(d => d.SourceWord.Title))
                .ForMember(s => s.SourceWordId, o => o.MapFrom(d => d.SourceWordId))
                .ForMember(s => s.Links, o => o.Ignore())
                .ReverseMap()
                .ForMember(s => s.RelationType, o => o.MapFrom(d => (RelationType)d.RelationTypeId));


            CreateMap<Author, AuthorView>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.BookCount, o => o.MapFrom(s => s.BookCount))
                .ForMember(d => d.Links, o => o.Ignore())
                .ReverseMap()
                .ForMember(d => d.ImageId, o => o.Ignore())
                .ForMember(d => d.BookCount, o => o.Ignore());

            CreateMap<Category, CategoryView>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Links, o => o.Ignore())
                .ForMember(d => d.BookCount, o => o.MapFrom(s => s.BookCount))
                .ReverseMap()
                    .ForMember(d => d.BookCount, o => o.MapFrom(s => s.BookCount));

            CreateMap<Series, SeriesView>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
                .ForMember(d => d.BookCount, o => o.MapFrom(s => s.BookCount))
                .ForMember(d => d.Links, o => o.Ignore())
                .ReverseMap();

            CreateMap<Book, BookView>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
                .ForMember(d => d.AuthorId, o => o.MapFrom(s => s.AuthorId))
                .ForMember(d => d.AuthorName, o => o.MapFrom(s => s.AuthorName))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
                .ForMember(d => d.IsPublic, o => o.MapFrom(s => s.IsPublic))
                .ForMember(d => d.Language, o => o.MapFrom(s => (int)s.Language))
                .ForMember(d => d.SeriesId, o => o.MapFrom(s => (s.SeriesId)))
                .ForMember(d => d.SeriesName, o => o.MapFrom(s => s.SeriesName))
                .ForMember(d => d.SeriesIndex, o => o.MapFrom(s => s.SeriesIndex))
                .ForMember(d => d.Links, o => o.Ignore())
                .ForMember(d => d.DateAdded, o => o.MapFrom(s => s.DateAdded))
                .ForMember(d => d.DateUpdated, o => o.MapFrom(s => s.DateUpdated))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status))
                .ForMember(d => d.Copyrights, o => o.MapFrom(s => s.Copyrights))
                .ForMember(d => d.YearPublished, o => o.MapFrom(s => s.YearPublished))
                .ReverseMap()
                .ForMember(d => d.Language, o => o.MapFrom(s => (Languages)s.Language))
                .ForMember(d => d.Status, o => o.MapFrom(s => (BookStatuses)s.Status))
                .ForMember(d => d.Copyrights, o => o.MapFrom(s => (CopyrightStatuses)s.Copyrights))
                .ForMember(d => d.ImageId, o => o.Ignore());

            CreateMap<Chapter, ChapterView>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
                .ForMember(d => d.ChapterNumber, o => o.MapFrom(s => s.ChapterNumber))
                .ForMember(d => d.BookId, o => o.MapFrom(s => s.BookId))
                .ForMember(d => d.Links, o => o.Ignore())
                .ReverseMap()
                    .ForMember(d => d.HasContents, o => o.Ignore());


            CreateMap<ChapterContent, ChapterContentView>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Contents, o => o.MapFrom(s => s.Content))
                .ForMember(d => d.ChapterId, o => o.MapFrom(s => s.ChapterId))
                .ForMember(d => d.Links, o => o.Ignore())
                .ReverseMap();
        }
    }
}