using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Mappings
{
    public static class ChapterMapper
    {
        public static ChapterView Map(this ChapterModel source)
            => new ChapterView
            {
                Id = source.Id,
                Title = source.Title,
                ChapterNumber = source.ChapterNumber,
                BookId = source.BookId,
                Status = source.Status.ToDescription(),
                WriterAccountId = source.WriterAccountId,
                WriterAccountName = source.WriterAccountName,
                WriterAssignTimeStamp = source.WriterAssignTimeStamp,
                ReviewerAccountId = source.ReviewerAccountId,
                ReviewerAccountName = source.ReviewerAccountName,
                ReviewerAssignTimeStamp = source.ReviewerAssignTimeStamp
            };

        public static ChapterModel Map(this ChapterView source)
            => new ChapterModel
            {
                Id = source.Id,
                Title = source.Title,
                ChapterNumber = source.ChapterNumber,
                BookId = source.BookId,
                Status = source.Status.ToEnum(EditingStatus.Available),
                WriterAccountId = source.WriterAccountId,
                WriterAccountName = source.WriterAccountName,
                WriterAssignTimeStamp = source.WriterAssignTimeStamp,
                ReviewerAccountId = source.ReviewerAccountId,
                ReviewerAccountName = source.ReviewerAccountName,
                ReviewerAssignTimeStamp = source.ReviewerAssignTimeStamp,
            };

        public static ChapterContentView Map(this ChapterContentModel source)
            => new ChapterContentView
            {
                Id = source.Id,
                ChapterId = source.ChapterId,
                BookId = source.BookId,
                ChapterNumber = source.ChapterNumber,
                Language = source.Language,
                Text = source.Text
            };

        public static ChapterContentModel Map(this ChapterContentView source)
            => new ChapterContentModel
            {
                Id = source.Id,
                ChapterId = source.ChapterId,
                ChapterNumber = source.ChapterNumber,
                BookId = source.BookId,
                Language = source.Language,
                Text = source.Text
            };
    }
}
