using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Paramore.Brighter;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class BindBookRequest : RequestBase
{
    public BindBookRequest(int libraryId, int bookId)
    {
        LibraryId = libraryId;
        BookId = bookId;
    }

    public string Result { get; set; }
    public int LibraryId { get; }
    public int BookId { get; }
}

public class BindBookRequestHandler : RequestHandlerAsync<BindBookRequest>
{
    private readonly IBookPageRepository _bookPageRepository;

    public BindBookRequestHandler(IBookPageRepository bookPageRepository)
    {
        _bookPageRepository = bookPageRepository;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin)]
    public override async Task<BindBookRequest> HandleAsync(BindBookRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var pages = await _bookPageRepository.GetAllPagesByBook(command.LibraryId, command.BookId, cancellationToken);

        var builder = new StringBuilder();
        foreach (var page in pages.OrderBy(p => p.SequenceNumber))
        {
            builder.AppendLine(page.Text);
        }

        command.Result = builder.ToString();

        return await base.HandleAsync(command, cancellationToken);
    }
}
