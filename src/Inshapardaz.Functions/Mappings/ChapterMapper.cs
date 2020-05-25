using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Functions.Views.Library;

namespace Inshapardaz.Functions.Mappings
{
    public static class ChapterMapper
    {
        public static ChapterView Map(this ChapterModel source)
            => new ChapterView
            {
                Id = source.Id,
                Title = source.Title,
                ChapterNumber = source.ChapterNumber,
                BookId = source.BookId
            };

        public static ChapterModel Map(this ChapterView source)
            => new ChapterModel
            {
                Id = source.Id,
                Title = source.Title,
                ChapterNumber = source.ChapterNumber,
                BookId = source.BookId
            };

        public static ChapterContentView Map(this ChapterContentModel source)
            => new ChapterContentView
            {
                Id = source.Id,
                ChapterId = source.ChapterId,
                BookId = source.BookId,
                Language = source.Language,
                MimeType = source.MimeType
            };

        public static ChapterContentModel Map(this ChapterContentView source)
            => new ChapterContentModel
            {
                Id = source.Id,
                ChapterId = source.ChapterId
            };
    }
}
