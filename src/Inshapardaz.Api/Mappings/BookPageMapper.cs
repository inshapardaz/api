using Inshapardaz.Api.Views.Library;
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
                Status = view.Status,
                UserId = view.UserId,
                AssignTimeStamp = view.AssignTimeStamp
            };
        }

        public static BookPageView Map(this BookPageModel model)
        {
            return new BookPageView
            {
                BookId = model.BookId,
                Text = model.Text,
                SequenceNumber = model.SequenceNumber,
                Status = model.Status,
                UserId = model.UserId,
                AssignTimeStamp = model.AssignTimeStamp
            };
        }
    }
}
