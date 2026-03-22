namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public abstract class BookRequest(int libraryId, int bookId) : LibraryBaseCommand(libraryId)
{
    public int BookId { get; set; } = bookId;
}
