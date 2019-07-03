using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;

namespace Inshapardaz.Functions
{
    public static class MappingProfile
    {
        #region File
        public static FileView Map(this File source) 
            =>  new FileView
            {
                Id = source.Id,
                MimeType = source.MimeType,
                FileName = source.FileName,
                DateCreated = source.DateCreated,
            };

        public static File Map(this FileView source) 
            =>  new File
            {
                Id = source.Id,
                MimeType = source.MimeType,
                FileName = source.FileName,
                DateCreated = source.DateCreated
            };
        #endregion
        
        #region Category
        public static CategoryView Map(this Category source)
            => new CategoryView
            {
                Id = source.Id,
                Name = source.Name,
                BookCount =  source.BookCount
            };

        public static Category Map(this CategoryView source)
            => new Category
            {
                Id = source.Id,
                Name = source.Name
            };
        #endregion
        
        #region  Series
        public static SeriesView Map(this Series source)
            => new SeriesView
            {
                Id = source.Id,
                Name = source.Name,
                Description = source.Description,
                BookCount = source.BookCount
            };

        public static Series Map(this SeriesView source)
            => new Series
            {
                Id = source.Id,
                Name = source.Name,
                Description = source.Description
            };

        
        #endregion
        
        #region Author
        public static AuthorView Map(this Author source)
            => new AuthorView
            {
                Id = source.Id,
                Name = source.Name,
                BookCount = source.BookCount
            };

        public static Author Map(this AuthorView source)
            => new Author
            {
                Id = source.Id,
                Name = source.Name
            };
        #endregion
        
        #region  Book
        public static BookView Map(this Book source)
            => new BookView
            {
                Id = source.Id,
                Title = source.Title,
                Description = source.Description,
                AuthorId = source.AuthorId,
                AuthorName = source.AuthorName, 
                IsPublic = source.IsPublic,
                Language = (int)source.Language,
                DateAdded = source.DateAdded,
                DateUpdated = source.DateUpdated,
                SeriesId = source.SeriesId,
                SeriesName = source.SeriesName,
                SeriesIndex = source.SeriesIndex,
                Copyrights = (int)source.Copyrights,
                Status = (int)source.Status,
                YearPublished = source.YearPublished,
                IsPublished = source.IsPublished
            };

        public static Book Map(this BookView source)
            => new Book
            {
                Id = source.Id,
                Title = source.Title,
                Description = source.Description,
                AuthorId = source.AuthorId,
                IsPublic = source.IsPublic,
                Language = (Languages) source.Language,
                DateAdded = source.DateAdded,
                DateUpdated = source.DateUpdated,
                SeriesId = source.SeriesId,
                SeriesIndex = source.SeriesIndex,
                Copyrights = (CopyrightStatuses) source.Copyrights,
                Status = (BookStatuses) source.Status,
                YearPublished = source.YearPublished,
                IsPublished = source.IsPublished
            };
        #endregion
        
        //public MappingProfile()
        //{
            // CreateMap<File, FileView>()
            //     .ForMember(s => s.Id, o => o.MapFrom(d => d.Id))
            //     .ForMember(s => s.DateCreated, o => o.MapFrom(d => d.DateCreated))
            //     .ForMember(s => s.FileName, o => o.MapFrom(d => d.FileName))
            //     .ForMember(s => s.MimeType, o => o.MapFrom(d => d.MimeType))
            //     .ForMember(d => d.Links, o => o.Ignore())
            //     .ReverseMap();

            // CreateMap<Dictionary, DictionaryView>()
            //     .ForMember(s => s.Id, o => o.MapFrom(d => d.Id))
            //     .ForMember(s => s.Name, o => o.MapFrom(d => d.Name))
            //     .ForMember(s => s.Language, o => o.MapFrom(d => d.Language))
            //     .ForMember(s => s.IsPublic, o => o.MapFrom(d => d.IsPublic))
            //     .ForMember(s => s.UserId, o => o.MapFrom(d => d.UserId))
            //     .ForMember(d => d.Links, o => o.Ignore())
            //     .ForMember(d => d.Indexes, o => o.Ignore())
            //     .ForMember(d => d.WordCount, o => o.MapFrom(s => s.WordCount))
            //     .ReverseMap();

            // CreateMap<Word, WordView>()
            //     .ForMember(s => s.Id, o => o.MapFrom(d => d.Id))
            //     .ForMember(s => s.Title, o => o.MapFrom(d => d.Title))
            //     .ForMember(s => s.TitleWithMovements, o => o.MapFrom(d => d.TitleWithMovements))
            //     .ForMember(s => s.Description, o => o.MapFrom(d => d.Description))
            //     .ForMember(s => s.Pronunciation, o => o.MapFrom(d => d.Pronunciation))
            //     .ForMember(s => s.Attributes, o => o.MapFrom(d => d.Attributes))
            //     .ForMember(s => s.AttributeValue, o => o.MapFrom(d => (int)d.Attributes))
            //     .ForMember(s => s.Language, o => o.MapFrom(d => d.Language))
            //     .ForMember(s => s.LanguageId, o => o.MapFrom(d => (int)d.Language))
            //     .ForMember(s => s.Links, o => o.Ignore())
            //     .ReverseMap()
            //     .ForMember(s => s.Relations, o => o.Ignore())
            //     .ForMember(s => s.Attributes, o => o.MapFrom(d => (GrammaticalType)d.AttributeValue))
            //     .ForMember(s => s.Language, o => o.MapFrom(d => (Languages)d.LanguageId))
            //     .ForMember(s => s.Meaning, o => o.Ignore())
            //     .ForMember(s => s.Translation, o => o.Ignore())
            //     .ForMember(s => s.DictionaryId, o => o.Ignore());

            // CreateMap<Word, SpellingOption>()
            //     .ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
            //     .ForMember(d => d.Links, o => o.Ignore());


            // CreateMap<Meaning, MeaningView>()
            //     .ForMember(s => s.Id, o => o.MapFrom(d => d.Id))
            //     .ForMember(s => s.Context, o => o.MapFrom(d => d.Context))
            //     .ForMember(s => s.Value, o => o.MapFrom(d => d.Value))
            //     .ForMember(s => s.Example, o => o.MapFrom(d => d.Example))
            //     .ForMember(s => s.WordId, o => o.MapFrom(d => d.WordId))
            //     .ForMember(s => s.Links, o => o.Ignore())
            //     .ReverseMap();

            // CreateMap<Translation, TranslationView>()
            //     .ForMember(s => s.Id, o => o.MapFrom(d => d.Id))
            //     .ForMember(s => s.Language, o => o.MapFrom(d => d.Language))
            //     .ForMember(s => s.LanguageId, o => o.MapFrom(d => (int)d.Language))
            //     .ForMember(s => s.Value, o => o.MapFrom(d => d.Value))
            //     .ForMember(s => s.WordId, o => o.MapFrom(d => d.WordId))
            //     .ForMember(s => s.Links, o => o.Ignore())
            //     .ForMember(s => s.IsTranspiling, o => o.MapFrom(s => s.IsTrasnpiling))
            //     .ReverseMap()
            //     .ForMember(s => s.Language, o => o.MapFrom(d => (Languages)d.LanguageId));

            // CreateMap<WordRelation, RelationshipView>()
            //     .ForMember(s => s.Id, o => o.MapFrom(d => d.Id))
            //     .ForMember(s => s.RelatedWordId, o => o.MapFrom(d => d.RelatedWordId))
            //     .ForMember(s => s.RelationType, o => o.MapFrom(d => d.RelationType))
            //     .ForMember(s => s.RelationTypeId, o => o.MapFrom(d => (int)d.RelationType))
            //     .ForMember(s => s.RelatedWord, o => o.MapFrom(d => d.RelatedWord.Title))
            //     .ForMember(s => s.SourceWord, o => o.MapFrom(d => d.SourceWord.Title))
            //     .ForMember(s => s.SourceWordId, o => o.MapFrom(d => d.SourceWordId))
            //     .ForMember(s => s.Links, o => o.Ignore())
            //     .ReverseMap()
            //     .ForMember(s => s.RelationType, o => o.MapFrom(d => (RelationType)d.RelationTypeId));
            
            // CreateMap<Chapter, ChapterView>()
            //     .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            //     .ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
            //     .ForMember(d => d.ChapterNumber, o => o.MapFrom(s => s.ChapterNumber))
            //     .ForMember(d => d.BookId, o => o.MapFrom(s => s.BookId))
            //     .ForMember(d => d.Links, o => o.Ignore())
            //     .ReverseMap()
            //         .ForMember(d => d.Contents, o => o.Ignore());


            // CreateMap<ChapterContent, ChapterContentView>()
            //     .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            //     .ForMember(d => d.Contents, o => o.MapFrom(s => s.Content))
            //     .ForMember(d => d.ChapterId, o => o.MapFrom(s => s.ChapterId))
            //     .ForMember(d => d.Links, o => o.Ignore())
            //     .ReverseMap();

            // CreateMap<Periodical, PeriodicalView>()
            //     .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            //     .ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
            //     .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
            //     .ForMember(d => d.Links, o => o.Ignore())
            //     .ReverseMap()
            //         .ForMember(d => d.CategoryId, o => o.Ignore());

            // CreateMap<Issue, IssueView>()
            //     .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            //     .ForMember(d => d.IssueDate, o => o.MapFrom(s => s.IssueDate))
            //     .ForMember(d => d.IssueNumber, o => o.MapFrom(s => s.IssueNumber))
            //     .ForMember(d => d.VolumeNumber, o => o.MapFrom(s => s.VolumeNumber))
            //     .ForMember(d => d.Links, o => o.Ignore())
            //     .ReverseMap()
            //         .ForMember(d => d.ImageId, o => o.Ignore())
            //         .ForMember(d => d.PeriodicalId, o => o.Ignore())
            //         .ForMember(d => d.Periodical, o => o.Ignore());
        //}
    }
}