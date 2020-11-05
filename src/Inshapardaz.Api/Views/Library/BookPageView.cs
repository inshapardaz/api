namespace Inshapardaz.Api.Views.Library
{
    public class BookPageView : ViewWithLinks
    {
        public int SequenceNumber { get; set; }

        public string Text { get; set; }
        public int BookId { get; set; }
    }
}
