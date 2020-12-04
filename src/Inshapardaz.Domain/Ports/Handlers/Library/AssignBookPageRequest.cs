using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class AssignBookPageRequest : LibraryBaseCommand
    {
        public AssignBookPageRequest(int libraryId, int bookId, int sequenceNumber, PageStatuses status, int? accountId)
        : base(libraryId)
        {
            BookId = bookId;
            SequenceNumber = sequenceNumber;
            Status = status;
            AccountId = accountId;
        }

        public BookPageModel Result { get; set; }
        public int BookId { get; set; }
        public int SequenceNumber { get; set; }
        public PageStatuses Status { get; set; }
        public int? AccountId { get; private set; }
        public int? AssignedAccountId { get; set; }
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

            command.Result = await _bookPageRepository.UpdatePageAssignment(command.LibraryId, command.BookId, command.SequenceNumber, command.Status, command.AssignedAccountId, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
