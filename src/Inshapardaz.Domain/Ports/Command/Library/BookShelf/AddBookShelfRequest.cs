﻿using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.BookShelf;

public class AddBookShelfRequest : LibraryBaseCommand
{
    public AddBookShelfRequest(int libraryId, BookShelfModel bookShelf)
        : base(libraryId)
    {
        BookShelf = bookShelf;
    }

    public BookShelfModel BookShelf { get; }
    public BookShelfModel Result { get; set; }
}

public class AddBookShelfRequestHandler : RequestHandlerAsync<AddBookShelfRequest>
{
    private readonly IBookShelfRepository _bookShelfRepository;
    private readonly IUserHelper _userHelper;

    public AddBookShelfRequestHandler(IBookShelfRepository bookShelfRepository, IUserHelper userHelper)
    {
        _bookShelfRepository = bookShelfRepository;
        _userHelper = userHelper;
    }

    [LibraryAuthorize(1)]
    public override async Task<AddBookShelfRequest> HandleAsync(AddBookShelfRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        command.BookShelf.AccountId = _userHelper.AccountId.Value;
        command.Result = await _bookShelfRepository.AddBookShelf(command.LibraryId, command.BookShelf, cancellationToken);
        return await base.HandleAsync(command, cancellationToken);
    }
}
