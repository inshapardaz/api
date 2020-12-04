using Inshapardaz.Domain.Models.Handlers.Library;

namespace Inshapardaz.Domain.Models.Library
{
    public abstract class BookRequest : LibraryBaseCommand
    {
        protected BookRequest(int libraryId, int bookId)
            : base(libraryId)
        {
            BookId = bookId;
        }

        public int BookId { get; set; }
    }
}
