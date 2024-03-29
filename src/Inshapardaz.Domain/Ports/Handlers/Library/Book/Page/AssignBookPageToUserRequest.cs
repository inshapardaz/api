﻿using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Book.Page
{
    public class AssignBookPageRequest : LibraryBaseCommand
    {
        public AssignBookPageRequest(int libraryId, int bookId, int sequenceNumber, int? accountId)
        : base(libraryId)
        {
            BookId = bookId;
            SequenceNumber = sequenceNumber;
            AccountId = accountId;
        }

        public BookPageModel Result { get; set; }
        public int BookId { get; set; }
        public int SequenceNumber { get; set; }
        public int? AccountId { get; private set; }
    }

    public class AssignBookPageRequestHandler : RequestHandlerAsync<AssignBookPageRequest>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBookPageRepository _bookPageRepository;

        public AssignBookPageRequestHandler(IBookRepository bookRepository,
                                         IBookPageRepository bookPageRepository)
        {
            _bookRepository = bookRepository;
            _bookPageRepository = bookPageRepository;
        }

        public override async Task<AssignBookPageRequest> HandleAsync(AssignBookPageRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var page = await _bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookId, command.SequenceNumber, cancellationToken);
            if (page == null)
            {
                throw new BadRequestException();
            }

            if (page.Status == EditingStatus.Available || page.Status == EditingStatus.Typing)
            {
                command.Result = await _bookPageRepository.UpdateWriterAssignment(command.LibraryId, command.BookId, command.SequenceNumber, command.AccountId, cancellationToken);
            }
            else if (page.Status == EditingStatus.Typed || page.Status == EditingStatus.InReview)
            {
                command.Result = await _bookPageRepository.UpdateReviewerAssignment(command.LibraryId, command.BookId, command.SequenceNumber, command.AccountId, cancellationToken);
            }
            else
            {
                throw new BadRequestException("Page status does not allow it to be assigned");
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
