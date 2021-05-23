﻿using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class AssignBookPageToUserRequest : LibraryBaseCommand
    {
        public AssignBookPageToUserRequest(int libraryId, int bookId, int sequenceNumber, int? accountId)
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

    public class AssignBookPageToUserRequestHandler : RequestHandlerAsync<AssignBookPageToUserRequest>
    {
        private readonly IBookPageRepository _bookPageRepository;

        public AssignBookPageToUserRequestHandler(IBookPageRepository bookPageRepository)
        {
            _bookPageRepository = bookPageRepository;
        }

        public override async Task<AssignBookPageToUserRequest> HandleAsync(AssignBookPageToUserRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var page = await _bookPageRepository.GetPageBySequenceNumber(command.LibraryId, command.BookId, command.SequenceNumber, cancellationToken);
            if (page == null)
            {
                throw new BadRequestException();
            }

            command.Result = await _bookPageRepository.UpdatePageAssignment(command.LibraryId, command.BookId, command.SequenceNumber, command.AccountId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
