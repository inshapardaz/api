using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;
using System.Text;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class BindBookRequest(int libraryId, int bookId) : RequestBase
{
    public string Result { get; set; }
    public int LibraryId { get; } = libraryId;
    public int BookId { get; } = bookId;
}

public class BindBookRequestHandler(IBookPageRepository bookPageRepository) : RequestHandlerAsync<BindBookRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin)]
    public override async Task<BindBookRequest> HandleAsync(BindBookRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var pages = await bookPageRepository.GetAllPagesByBook(command.LibraryId, command.BookId, cancellationToken);

        var builder = new StringBuilder();
        foreach (var page in pages.OrderBy(p => p.SequenceNumber))
        {
            builder.AppendLine(page.Text);
        }

        command.Result = builder.ToString();

        return await base.HandleAsync(command, cancellationToken);
    }
}
