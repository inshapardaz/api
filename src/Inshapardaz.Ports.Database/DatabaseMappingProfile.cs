using System.Linq;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Entities.Library;

namespace Inshapardaz.Ports.Database
{
    public static class DatabaseMappingProfile
    {
        #region Dictionary
        public static Entities.Dictionary.Dictionary Map(this Dictionary source)
        => source == null ? null : new Entities.Dictionary.Dictionary
        {
            Id = source.Id,
            Name = source.Name,
            Language = source.Language,
            IsPublic = source.IsPublic,
            UserId = source.UserId
        };

        public static Dictionary Map(this Entities.Dictionary.Dictionary source)
        => source == null ? null : new Dictionary
        {
            Id = source.Id,
            Name = source.Name,
            Language = source.Language,
            IsPublic = source.IsPublic,
            UserId = source.UserId
        };

        #endregion

        #region DictionaryDownload
        public static Entities.Dictionary.DictionaryDownload Map(this DictionaryDownload source) 
        => source == null ? null : new Entities.Dictionary.DictionaryDownload
        {
            Id = source.Id,
            DictionaryId = source.DictionaryId,
            MimeType = source.MimeType
        };

        public static DictionaryDownload Map(this Entities.Dictionary.DictionaryDownload source)
        => source == null ? null : new DictionaryDownload
        {
            Id = source.Id,
            DictionaryId = source.DictionaryId,
            MimeType = source.MimeType,
            File = source.File?.FileName
        };
        #endregion

        #region File
        public static Entities.File Map(this File source) 
        => source == null ? null : new Entities.File
        {
            Id = source.Id,
            MimeType = source.MimeType,
            FileName = source.FileName,
            DateCreated = source.DateCreated,
            FilePath = source.FilePath
        };

        public static File Map(this Entities.File source) 
        => source == null ? null : new File
        {
            Id = source.Id,
            MimeType = source.MimeType,
            FileName = source.FileName,
            DateCreated = source.DateCreated,
            FilePath = source.FilePath
        };

        #endregion
        
        #region Word

        public static Entities.Dictionary.Word Map(this Word source)
        => source == null ? null : new Entities.Dictionary.Word
        {
            Id = source.Id,
            Title = source.Title,
            TitleWithMovements = source.TitleWithMovements,
            Attributes = source.Attributes,
            Description = source.Description,
            DictionaryId = source.DictionaryId,
            Language = source.Language,
            Pronunciation = source.Pronunciation,
            Meaning = source.Meaning?.Select(m => m.Map())?.ToArray(),
            Translation = source.Translation?.Select(t => t.Map())?.ToArray(),
            WordRelationRelatedWord = source.WordRelationRelatedWord?.Select(m => m.Map())?.ToArray(),
            WordRelationSourceWord = source.WordRelationSourceWord?.Select(m => m.Map())?.ToArray()
        };

        public static Word Map(this Entities.Dictionary.Word source)
        => source == null ? null : new Word
        {
            Id = source.Id,
            Title = source.Title,
            TitleWithMovements = source.TitleWithMovements,
            Attributes = source.Attributes,
            Description = source.Description,
            DictionaryId = source.DictionaryId,
            Language = source.Language,
            Pronunciation = source.Pronunciation,
            Meaning = source.Meaning?.Select(m => m.Map())?.ToArray(),
            Translation = source.Translation?.Select(t => t.Map())?.ToArray(),
            WordRelationRelatedWord = source.WordRelationRelatedWord.Select(m => m.Map())?.ToArray(),
            WordRelationSourceWord = source.WordRelationSourceWord?.Select(m => m.Map())?.ToArray()
        };
        
        #endregion

        #region Meaning

        public static Entities.Dictionary.Meaning Map(this Meaning source)
        => source == null ? null : new Entities.Dictionary.Meaning
        {
            Id = source.Id,
            Context = source.Context,
            Example = source.Example,
            Value = source.Value,
            WordId = source.WordId
        };

        public static Meaning Map(this Entities.Dictionary.Meaning source)
        => source == null ? null : new Meaning
        {
            Id = source.Id,
            Context = source.Context,
            Example = source.Example,
            Value = source.Value,
            WordId = source.WordId
        };

        #endregion

        #region Translation
        public static Entities.Dictionary.Translation Map(this Translation source)
        => source == null ? null : new Entities.Dictionary.Translation
        {
            Id = source.Id,
            IsTrasnpiling = source.IsTrasnpiling,
            Language = source.Language,
            Value = source.Value,
            WordId = source.WordId
        };

        public static Translation Map(this Entities.Dictionary.Translation source)
        => source == null ? null : new Translation
        {
            Id = source.Id,
            IsTrasnpiling = source.IsTrasnpiling,
            Language = source.Language,
            Value = source.Value,
            WordId = source.WordId
        };

        #endregion

        #region WordRelation

        public static Entities.Dictionary.WordRelation Map(this WordRelation source)
        => source == null ? null : new Entities.Dictionary.WordRelation
        {
            Id = source.Id,
            RelatedWordId = source.RelatedWordId,
            RelationType = source.RelationType,
            SourceWordId = source.SourceWordId
        };

        public static WordRelation Map(this Entities.Dictionary.WordRelation source)
        => source == null ? null : new WordRelation
        {
            Id = source.Id,
            RelatedWordId = source.RelatedWordId,
            RelationType = source.RelationType,
            SourceWordId = source.SourceWordId,

        };

        #endregion
        
        #region Category
        public static Entities.Library.Category Map(this Category source)
        => source == null ? null : new Entities.Library.Category
        {
            Id = source.Id,
            Name = source.Name
        };

        public static Category Map(this Entities.Library.Category source)
        => source == null ? null : new Category
        {
            Id = source.Id,
            Name = source.Name
        };

        public static Category MapFromBookCategory(this Entities.Library.BookCategory source)
        => source == null ? null : new Category
        {
            Id = source.CategoryId,
            Name = source.Category?.Name
        };
        #endregion
        
        #region  Series
        public static Entities.Library.Series Map(this Series source)
        => source == null ? null : new Entities.Library.Series
        {
            Id = source.Id,
            Name = source.Name,
            Description = source.Description,
            ImageId = source.ImageId
        };

        public static Series Map(this Entities.Library.Series source)
        => source == null ? null : new Series
        {
            Id = source.Id,
            Name = source.Name,
            Description = source.Description,
            ImageId = source.ImageId
        };

        #endregion

        #region Author
        public static Entities.Library.Author Map(this Author source)
        => source == null ? null : new Entities.Library.Author
        {
            Id = source.Id,
            Name = source.Name,
            ImageId = source.ImageId
        };

        public static Author Map(this Entities.Library.Author source)
        => source == null ? null : new Author
        {
            Id = source.Id,
            Name = source.Name,
            ImageId = source.ImageId
        };
        #endregion

        #region  Book
        public static Entities.Library.Book Map(this Book source)
        => source == null ? null : new Entities.Library.Book
        {
                Id = source.Id,
                Title = source.Title,
                Description = source.Description,
                AuthorId = source.AuthorId,
                IsPublic = source.IsPublic,
                Language = source.Language,
                ImageId = source.ImageId,
                DateAdded = source.DateAdded,
                DateUpdated = source.DateUpdated,
                SeriesId = source.SeriesId,
                SeriesIndex = source.SeriesIndex,
                Copyrights = source.Copyrights,
                Status = source.Status,
                YearPublished = source.YearPublished,
                IsPublished = source.IsPublished
        };

        public static Book Map(this Entities.Library.Book source)
        => source == null ? null : new Book
        {
                Id = source.Id,
                Title = source.Title,
                Description = source.Description,
                AuthorId = source.AuthorId,
                IsPublic = source.IsPublic,
                Language = source.Language,
                Categories = source.BookCategory.Select(c => c.MapFromBookCategory()).ToArray(),
                ImageId = source.ImageId,
                DateAdded = source.DateAdded,
                DateUpdated = source.DateUpdated,
                SeriesId = source.SeriesId,
                SeriesIndex = source.SeriesIndex,
                Copyrights = source.Copyrights,
                Status = source.Status,
                YearPublished = source.YearPublished,
                IsPublished = source.IsPublished,
                SeriesName = source.Series?.Name,
                AuthorName = source.Author?.Name
        };
        #endregion

        #region Chapter
        public static Entities.Library.Chapter Map(this Chapter source)
        => source == null ? null : new Entities.Library.Chapter
        {
            Id = source.Id,
            Title = source.Title,
            BookId = source.BookId,
            ChapterNumber = source.ChapterNumber
        };

        public static Chapter Map(this Entities.Library.Chapter source)
        => source == null ? null : new Chapter
        {
            Id = source.Id,
            Title = source.Title,
            BookId = source.BookId,
            ChapterNumber =source.ChapterNumber
        };
        #endregion

        #region ChaterContent
        public static Entities.Library.ChapterContent Map(this ChapterContent source)
        => source == null ? null : new Entities.Library.ChapterContent
        {
            Id = source.Id,
            ChapterId = source.ChapterId,
            MimeType = source.MimeType
        };

        public static ChapterContent Map(this Entities.Library.ChapterContent source)
        => source == null ? null : new ChapterContent
        {
            Id = source.Id,
            ChapterId = source.ChapterId,
            MimeType = source.MimeType,
            BookId = source.Chapter?.BookId??0
        };

        #endregion

        #region Periodical
        public static Entities.Library.Periodical Map(this Periodical source)
        => source == null ? null : new Entities.Library.Periodical
        {
            Id = source.Id,
            Title = source.Title,
            Description = source.Description,
            ImageId = source.ImageId,
            CategoryId = source.CategoryId
        };

        public static Periodical Map(this Entities.Library.Periodical source)
        => source == null ? null : new Periodical
        {
            Id = source.Id,
            Title = source.Title,
            Description = source.Description,
            ImageId = source.ImageId,
            CategoryId = source.CategoryId
        };

        #endregion

        #region  Issue
        public static Entities.Library.Issue Map(this Issue source)
        => source == null ? null : new Entities.Library.Issue
        {
            Id = source.Id,
            IssueDate = source.IssueDate,
            IssueNumber = source.IssueNumber,
            VolumeNumber = source.VolumeNumber,
            ImageId = source.ImageId,
            PeriodicalId = source.PeriodicalId
        };

        public static Issue Map(this Entities.Library.Issue source)
        => source == null ? null : new Issue
        {
            Id = source.Id,
            IssueDate = source.IssueDate,
            IssueNumber = source.IssueNumber,
            VolumeNumber = source.VolumeNumber,
            ImageId = source.ImageId,
            PeriodicalId = source.PeriodicalId
        };
        #endregion

        #region Article
        public static Entities.Library.Article Map(this Article source)
        => source == null ? null : new Entities.Library.Article
        {
            Id = source.Id,
            SequenceNumber = source.SequenceNumber,
            SeriesName = source.SeriesName,
            SeriesIndex = source.SeriesIndex,
            AuthorId = source.AuthorId,
            IssueId = source.IssueId
        };

        public static Article Map(this Entities.Library.Article source)
        => source == null ? null : new Article
        {
            Id = source.Id,
            SequenceNumber = source.SequenceNumber,
            SeriesName = source.SeriesName,
            SeriesIndex = source.SeriesIndex,
            AuthorId = source.AuthorId,
            IssueId = source.IssueId
        };
        #endregion
    }
}
