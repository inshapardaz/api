using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class AssignChapterToUserRequest : LibraryBaseCommand
    {
        public AssignChapterToUserRequest(int libraryId, int bookId, int chapterNumber, int accountId, AssignmentTypes type)
        : base(libraryId)
        {
            BookId = bookId;
            ChapterNumber = chapterNumber;
            AccountId = accountId;
            AssignmentType = type;
        }

        public ChapterModel Result { get; set; }
        public int BookId { get; set; }
        public int ChapterNumber { get; set; }
        public int AccountId { get; private set; }
        public AssignmentTypes AssignmentType { get; }
    }

    public class AssignChapterToUserRequestHandler : RequestHandlerAsync<AssignChapterToUserRequest>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IChapterRepository _chapterRepository;

        public AssignChapterToUserRequestHandler(IAccountRepository accountRepository,
                                         IChapterRepository chapterRepository)
        {
            _accountRepository = accountRepository;
            _chapterRepository = chapterRepository;
        }

        public override async Task<AssignChapterToUserRequest> HandleAsync(AssignChapterToUserRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var account = await _accountRepository.GetLibraryAccountById(command.LibraryId, command.AccountId, cancellationToken);
            if (account.Role != Role.Admin  && account.Role != Role.LibraryAdmin && account.Role != Role.Writer)
            {
                throw new BadRequestException("user cannot be assigned chapter");
            }

            var chapter = await _chapterRepository.GetChapterById(command.LibraryId, command.BookId, command.ChapterNumber, cancellationToken);
            if (chapter == null)
            {
                throw new BadRequestException();
            }

            if (command.AssignmentType == AssignmentTypes.Write)
            {
                command.Result = await _chapterRepository.UpdateWriterAssignment(command.LibraryId, command.BookId, command.ChapterNumber, command.AccountId, cancellationToken);
            }
            else if (command.AssignmentType == AssignmentTypes.Review)
            {
                command.Result = await _chapterRepository.UpdateReviewerAssignment(command.LibraryId, command.BookId, command.ChapterNumber, command.AccountId, cancellationToken);
            }
            else
            {
                throw new BadRequestException("Chapter does not allow it to be assigned");
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
