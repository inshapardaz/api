using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Api.Views.Library;

namespace Inshapardaz.Api.Mappings
{
    public static class PeriosicalMapper
    {
        public static PeriodicalView Map(this PeriodicalModel source)
            => source == null ? null : new PeriodicalView
            {
                Id = source.Id,
                Title = source.Title,
                Description = source.Description,
                IssueCount = source.IssueCount
            };

        public static PeriodicalModel Map(this PeriodicalView source)
            => source == null ? null : new PeriodicalModel
            {
                Id = source.Id,
                Title = source.Title,
                Description = source.Description,
                IssueCount = source.IssueCount,
            };

        public static IssueView Map(this IssueModel source)
            => source == null ? null : new IssueView
            {
                Id = source.Id,
                PeriodicalId = source.PeriodicalId,
                VolumeNumber = source.VolumeNumber,
                IssueNumber = source.IssueNumber,
                IssueDate = source.IssueDate
            };

        public static IssueModel Map(this IssueView source)
            => source == null ? null : new IssueModel
            {
                Id = source.Id,
                PeriodicalId = source.PeriodicalId,
                VolumeNumber = source.VolumeNumber,
                IssueNumber = source.IssueNumber,
                IssueDate = source.IssueDate
            };

        public static IssueContentView Map(this IssueContentModel source)
            => new IssueContentView
            {
                Id = source.Id,
                PeriodicalId = source.PeriodicalId,
                IssueId = source.IssueId,
                Language = source.Language,
                MimeType = source.MimeType
            };

        public static IssueContentModel Map(this IssueContentView source)
            => new IssueContentModel
            {
                Id = source.Id,
                PeriodicalId = source.PeriodicalId,
                IssueId = source.IssueId,
                Language = source.Language,
                MimeType = source.MimeType
            };

        public static ArticleView Map(this ArticleModel source)
            => source == null ? null : new ArticleView
            {
                Id = source.Id,
                Title = source.Title,
                SequenceNumber = source.SequenceNumber,
                SeriesName = source.SeriesName,
                SeriesIndex = source.SeriesIndex
            };

        public static ArticleModel Map(this ArticleView source)
            => source == null ? null : new ArticleModel
            {
                Id = source.Id,
                Title = source.Title,
                SequenceNumber = source.SequenceNumber,
                SeriesName = source.SeriesName,
                SeriesIndex = source.SeriesIndex
            };

        public static ArticleContentView Map(this ArticleContentModel source)
            => new ArticleContentView
            {
                Id = source.Id,
                PeriodicalId = source.PeriodicalId,
                IssueId = source.IssueId,
                ArticleId = source.ArticleId,
                Language = source.Language,
                MimeType = source.MimeType
            };

        public static ArticleContentModel Map(this ArticleContentView source)
            => new ArticleContentModel
            {
                Id = source.Id,
                PeriodicalId = source.PeriodicalId,
                IssueId = source.IssueId,
                ArticleId = source.ArticleId,
                Language = source.Language,
                MimeType = source.MimeType
            };
    }
}
