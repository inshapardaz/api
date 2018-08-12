using AutoMapper;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;

namespace Inshapardaz.Ports.Database
{
    public class DatabaseMappingProfile : Profile
    {
        public DatabaseMappingProfile()
        {
            CreateMap<Dictionary, Entities.Dictionary.Dictionary>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Language, o => o.MapFrom(s => s.Language))
                .ForMember(d => d.IsPublic, o => o.MapFrom(s => s.IsPublic))
                .ForMember(d => d.UserId, o => o.MapFrom(s => s.UserId))
                .ForMember(d => d.Word, o => o.Ignore())
                .ReverseMap()
                .ForMember(d => d.Downloads, o => o.MapFrom(s => s.Downloads));

            CreateMap<DictionaryDownload, Entities.Dictionary.DictionaryDownload>()
                .ForMember(d => d.DictionaryId, o => o.MapFrom(s => s.DictionaryId))
                .ForMember(d => d.MimeType, o => o.MapFrom(s => s.MimeType))
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Dictionary, o => o.Ignore())
                .ForMember(d => d.File, o => o.MapFrom(s => s.File))
                .ForMember(d => d.FileId, o => o.Ignore())
                .ReverseMap();


            CreateMap<Word, Entities.Dictionary.Word>()
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

            CreateMap<Meaning, Entities.Dictionary.Meaning>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Context, o => o.MapFrom(s => s.Context))
                .ForMember(d => d.Example, o => o.MapFrom(s => s.Example))
                .ForMember(d => d.Value, o => o.MapFrom(s => s.Value))
                .ForMember(d => d.Word, o => o.Ignore())
                .ForMember(d => d.WordId, o => o.MapFrom(s => s.WordId))
                .ReverseMap()
                .ForMember(d => d.Word, o => o.Ignore());

            CreateMap<Translation, Entities.Dictionary.Translation>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.IsTrasnpiling, o => o.MapFrom(s => s.IsTrasnpiling))
                .ForMember(d => d.Language, o => o.MapFrom(s => s.Language))
                .ForMember(d => d.Value, o => o.MapFrom(s => s.Value))
                .ForMember(d => d.Word, o => o.Ignore())
                .ForMember(d => d.WordId, o => o.MapFrom(s => s.WordId))
                .ReverseMap()
                .ForMember(d => d.Word, o => o.Ignore());

            CreateMap<WordRelation, Entities.Dictionary.WordRelation>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.RelatedWord, o => o.Ignore())
                .ForMember(d => d.RelatedWordId, o => o.MapFrom(s => s.RelatedWordId))
                .ForMember(d => d.RelationType, o => o.MapFrom(s => s.RelationType))
                .ForMember(d => d.SourceWord, o => o.Ignore())
                .ForMember(d => d.SourceWordId, o => o.MapFrom(s => s.SourceWordId))
                .ReverseMap()
                .ForMember(d => d.RelatedWord, o => o.MapFrom(s => s.RelatedWord))
                .ForMember(d => d.SourceWord, o => o.MapFrom(s => s.SourceWord));

            CreateMap<Category, Entities.Library.Category>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.BookCategories, o => o.Ignore())
                .ReverseMap();


            CreateMap<Entities.Library.BookCategory, Category>()
                .ConvertUsing(bc => bc.Category.Map<Entities.Library.Category, Category>());

            CreateMap<Author, Entities.Library.Author>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.ImageId, o => o.MapFrom(s => s.ImageId))
                .ForMember(d => d.Books, o => o.Ignore())
                .ReverseMap();

            CreateMap<Book, Entities.Library.Book>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
                .ForMember(d => d.AuthorId, o => o.MapFrom(s => s.AuthorId))
                .ForMember(d => d.IsPublic, o => o.MapFrom(s => s.IsPublic))
                .ForMember(d => d.Language, o => o.MapFrom(s => s.Language))
                .ForMember(d => d.Chapters, o => o.Ignore())
                .ForMember(d => d.BookCategory, o => o.MapFrom(s => s.Categories))
                .ForMember(d => d.ImageId, o => o.MapFrom(s => s.ImageId))
                .ForMember(d => d.Author, o => o.Ignore())
                .ReverseMap()
                    .ForMember(d => d.AuthorName, o => o.MapFrom(s => s.Author.Name))
                    .ForMember(d => d.Categories, o => o.MapFrom(s => s.BookCategory));
            
            CreateMap<Chapter, Entities.Library.Chapter>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
                .ForMember(d => d.BookId, o => o.MapFrom(s => s.BookId))
                .ForMember(d => d.Content, o => o.Ignore())
                .ForMember(d => d.Book, o => o.Ignore())
                .ReverseMap();

            CreateMap<ChapterContent, Entities.Library.ChapterText>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.ChapterId, o => o.MapFrom(s => s.ChapterId))
                .ForMember(d => d.Content, o => o.MapFrom(s => s.Content))
                .ForMember(d => d.Chapter, o => o.Ignore())
                .ReverseMap()
                .ForMember(d => d.BookId, o => o.MapFrom(s => s.Chapter!= null ? s.Chapter.BookId : 0));
        }
    }
}
