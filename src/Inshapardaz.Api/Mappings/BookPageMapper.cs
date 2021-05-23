using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Api.Mappings
{
    public static class BookPageMapper
    {
        public static BookPageModel Map(this BookPageView view)
        {
            return new BookPageModel
            {
                BookId = view.BookId,
                Text = view.Text,
                SequenceNumber = view.SequenceNumber,
                Status = view.Status.ToEnum(PageStatuses.Available),
                AccountId = view.AccountId,
                AccountName = view.AccountName,
                AssignTimeStamp = view.AssignTimeStamp,
                ChapterId = view.ChapterId
            };
        }

        public static BookPageView Map(this BookPageModel model)
        {
            return new BookPageView
            {
                BookId = model.BookId,
                Text = model.Text,
                SequenceNumber = model.SequenceNumber,
                Status = model.Status.ToDescription(),
                AccountId = model.AccountId,
                AccountName = model.AccountName,
                AssignTimeStamp = model.AssignTimeStamp,
                ChapterId = model.ChapterId,
                ChapterTitle = model.ChapterTitle
            };
        }
    }
}
