using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Api.Mappings;

public static class BookPageMapper
{
    public static BookPageModel Map(this BookPageView view)
    {
        return new BookPageModel
        {
            BookId = view.BookId,
            Text = view.Text,
            SequenceNumber = view.SequenceNumber,
            Status = view.Status.ToEnum(EditingStatus.Available),
            WriterAccountId = view.WriterAccountId,
            WriterAccountName = view.WriterAccountName,
            WriterAssignTimeStamp = view.WriterAssignTimeStamp,
            ReviewerAccountId = view.ReviewerAccountId,
            ReviewerAccountName = view.ReviewerAccountName,
            ReviewerAssignTimeStamp = view.ReviewerAssignTimeStamp,
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
            WriterAccountId = model.WriterAccountId,
            WriterAccountName = model.WriterAccountName,
            WriterAssignTimeStamp = model.WriterAssignTimeStamp,
            ReviewerAccountId = model.ReviewerAccountId,
            ReviewerAccountName = model.ReviewerAccountName,
            ReviewerAssignTimeStamp = model.ReviewerAssignTimeStamp,
            ChapterId = model.ChapterId,
            ChapterNumber = model.ChapterNumber,  
            ChapterTitle = model.ChapterTitle
        };
    }
}
