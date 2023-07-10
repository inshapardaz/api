using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Mappings
{
    public static class ArticleMapper
    {
        public static ArticleView Map(this ArticleModel source)
            => source == null ? null : new ArticleView
            {
                Id = source.Id,
                Title = source.Title,
                Status = source.Status.ToDescription(),
                WriterAccountId = source.WriterAccountId,
                WriterAccountName = source.WriterAccountName,
                WriterAssignTimeStamp = source.WriterAssignTimeStamp,
                ReviewerAccountId = source.ReviewerAccountId,
                ReviewerAccountName = source.ReviewerAccountName,
                ReviewerAssignTimeStamp = source.ReviewerAssignTimeStamp,
            };

        public static ArticleModel Map(this ArticleView source)
            => source == null ? null : new ArticleModel
            {
                Id = source.Id,
                Title = source.Title,
                Status = source.Status.ToEnum(EditingStatus.Available),
                WriterAccountId = source.WriterAccountId,
                WriterAccountName = source.WriterAccountName,
                WriterAssignTimeStamp = source.WriterAssignTimeStamp,
                ReviewerAccountId = source.ReviewerAccountId,
                ReviewerAccountName = source.ReviewerAccountName,
                ReviewerAssignTimeStamp = source.ReviewerAssignTimeStamp
            };

        public static ArticleContentView Map(this ArticleContentModel source)
            => new ArticleContentView
            {
                Id = source.Id,
                BookId = source.BookId,
                ChapterNumber = source.ChapterNumber,
                Language = source.Language,
                Text = source.Text
            };

        public static ArticleContentModel Map(this ArticleContentView source)
            => new ArticleContentModel
            {
                Id = source.Id,
                BookId = source.BookId,
                ChapterNumber = source.ChapterNumber,
                Language = source.Language,
                Text = source.Text
            };
    }
}
