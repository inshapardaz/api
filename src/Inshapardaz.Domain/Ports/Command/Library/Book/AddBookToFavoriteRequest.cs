﻿using Inshapardaz.Domain.Adapters.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class AddBookToFavoriteRequest : LibraryBaseCommand
{
    public AddBookToFavoriteRequest(int libraryId, int bookId, int? accountId)
        : base(libraryId)
    {
        BookId = bookId;
        AccountId = accountId;
    }

    public int BookId { get; }
    public int? AccountId { get; }
}

public class AddBookToFavoriteRequestHandler : RequestHandlerAsync<AddBookToFavoriteRequest>
{
    private readonly IBookRepository _bookRepository;

    public AddBookToFavoriteRequestHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    [LibraryAuthorize(1)]
    public override async Task<AddBookToFavoriteRequest> HandleAsync(AddBookToFavoriteRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, command.AccountId, cancellationToken);
        if (book != null)
        {
            await _bookRepository.AddBookToFavorites(command.LibraryId, command.AccountId, command.BookId, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
