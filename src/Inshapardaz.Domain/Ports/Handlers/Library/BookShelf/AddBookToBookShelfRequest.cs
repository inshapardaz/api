﻿using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.BookShelf
{
    public class AddBookToBookShelfRequest : LibraryBaseCommand
    {
        public AddBookToBookShelfRequest(int libraryId, int bookShelfId, int bookId, int index, int accountId)
            : base(libraryId)
        {
            BookShelfId = bookShelfId;
            BookId = bookId;
            Index = index;
            AccountId = accountId;
        }

        public int BookShelfId { get; }
        public int BookId { get; }
        public int Index { get; }
        public int AccountId { get; }
    }

    public class AddBookToBookShelfRequestHandler : RequestHandlerAsync<AddBookToBookShelfRequest>
    {
        private readonly IBookShelfRepository _bookShelfRepository;
        private readonly IBookRepository _bookRepository;

        public AddBookToBookShelfRequestHandler(IBookShelfRepository bookShelfRepository, IBookRepository bookRepository)
        {
            _bookShelfRepository = bookShelfRepository;
            _bookRepository = bookRepository;
        }

        public override async Task<AddBookToBookShelfRequest> HandleAsync(AddBookToBookShelfRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, null, cancellationToken);
            if (book == null)
            {
                throw new BadRequestException("Book does not exist");
            }

            var bookShelf = await _bookShelfRepository.GetBookShelfById(command.LibraryId, command.BookShelfId, cancellationToken);

            if (bookShelf == null)
            {
                throw new BadRequestException("Bookshelf does not exist");
            }
            
            if (bookShelf.AccountId != command.AccountId)
            {
                throw new ForbiddenException();
            }

            await _bookShelfRepository.AddBookToBookShelf(command.LibraryId, command.BookShelfId, command.BookId, command.Index, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
