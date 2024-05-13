using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Api.Mappings
{
    public static class IssuePageMapper
    {
        public static IssuePageModel Map(this IssuePageView view)
        {
            return new IssuePageModel
            {
                PeriodicalId = view.PeriodicalId,
                VolumeNumber = view.VolumeNumber,
                IssueNumber = view.IssueNumber,
                Text = view.Text,
                SequenceNumber = view.SequenceNumber,
                Status = view.Status.ToEnum(EditingStatus.Available),
                WriterAccountId = view.WriterAccountId,
                WriterAccountName = view.WriterAccountName,
                WriterAssignTimeStamp = view.WriterAssignTimeStamp,
                ReviewerAccountId = view.ReviewerAccountId,
                ReviewerAccountName = view.ReviewerAccountName,
                ReviewerAssignTimeStamp = view.ReviewerAssignTimeStamp,
                ArticleName = view.ArticleName,
                ArticleId = view.ArticleId
            };
        }

        public static IssuePageView Map(this IssuePageModel model)
        {
            return new IssuePageView
            {
                PeriodicalId = model.PeriodicalId,
                VolumeNumber = model.VolumeNumber,
                IssueNumber = model.IssueNumber,
                Text = model.Text,
                SequenceNumber = model.SequenceNumber,
                Status = model.Status.ToDescription(),
                WriterAccountId = model.WriterAccountId,
                WriterAccountName = model.WriterAccountName,
                WriterAssignTimeStamp = model.WriterAssignTimeStamp,
                ReviewerAccountId = model.ReviewerAccountId,
                ReviewerAccountName = model.ReviewerAccountName,
                ReviewerAssignTimeStamp = model.ReviewerAssignTimeStamp,
                 ArticleName = model.ArticleName,
                ArticleId = model.ArticleId
            };
        }
    }
}
