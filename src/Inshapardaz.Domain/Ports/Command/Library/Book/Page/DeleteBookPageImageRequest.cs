﻿using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Ports.Command.File;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Page;

public class DeleteBookPageImageRequest : LibraryBaseCommand
{
    public DeleteBookPageImageRequest(int libraryId, int bookId, int sequenceNumber)
        : base(libraryId)
    {
        BookId = bookId;
        SequenceNumber = sequenceNumber;
    }

    public int BookId { get; }

    public int SequenceNumber { get; }
}

public class DeleteBookPageImageRequestHandler : RequestHandlerAsync<DeleteBookPageImageRequest>
{
    private readonly IBookPageRepository _bookPageRepository;
    private readonly IAmACommandProcessor _commandProcessor;

    public DeleteBookPageImageRequestHandler(IBookPageRepository bookPageRepository, 
        IAmACommandProcessor commandProcessor)
    {
        _bookPageRepository = bookPageRepository;
        _commandProcessor = commandProcessor;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<DeleteBookPageImageRequest> HandleAsync(DeleteBookPageImageRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var bookPage = await _bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookId, command.SequenceNumber, cancellationToken);

        if (bookPage != null)
        {
            await _commandProcessor.SendAsync(new DeleteFileCommand(bookPage.ImageId), cancellationToken: cancellationToken);
            await _bookPageRepository.DeletePageImage(command.LibraryId, command.BookId, command.SequenceNumber, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
