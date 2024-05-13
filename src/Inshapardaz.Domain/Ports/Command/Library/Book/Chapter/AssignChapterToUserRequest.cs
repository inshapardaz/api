using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Chapter;

public class AssignChapterToUserRequest : LibraryBaseCommand
{
    public AssignChapterToUserRequest(int libraryId, int bookId, int chapterNumber, int? accountId)
    : base(libraryId)
    {
        BookId = bookId;
        ChapterNumber = chapterNumber;
        AccountId = accountId;
    }

    public ChapterModel Result { get; set; }
    public int BookId { get; set; }
    public int ChapterNumber { get; set; }
    public int? AccountId { get; private set; }
}

public class AssignChapterToUserRequestHandler : RequestHandlerAsync<AssignChapterToUserRequest>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IChapterRepository _chapterRepository;
    private readonly IUserHelper _userHelper;

    public AssignChapterToUserRequestHandler(IAccountRepository accountRepository,
                                     IChapterRepository chapterRepository,
                                     IUserHelper userHelper)
    {
        _accountRepository = accountRepository;
        _chapterRepository = chapterRepository;
        _userHelper = userHelper;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AssignChapterToUserRequest> HandleAsync(AssignChapterToUserRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var accountToAssign = command.AccountId ?? _userHelper.Account.Id;
        if (!_userHelper.IsAdmin)
        {
            var account = await _accountRepository.GetLibraryAccountById(command.LibraryId, accountToAssign, cancellationToken);
            if (account.Role != Role.LibraryAdmin && account.Role != Role.Writer)
            {
                throw new BadRequestException("user cannot be assigned chapter");
            }
        }

        var chapter = await _chapterRepository.GetChapterById(command.LibraryId, command.BookId, command.ChapterNumber, cancellationToken);
        if (chapter == null)
        {
            throw new BadRequestException();
        }

        if (chapter.Status == EditingStatus.Available || chapter.Status == EditingStatus.Typing)
        {
            command.Result = await _chapterRepository.UpdateWriterAssignment(command.LibraryId, command.BookId, command.ChapterNumber, accountToAssign, cancellationToken);
        }
        else if (chapter.Status == EditingStatus.Typed || chapter.Status == EditingStatus.InReview)
        {
            command.Result = await _chapterRepository.UpdateReviewerAssignment(command.LibraryId, command.BookId, command.ChapterNumber, accountToAssign, cancellationToken);
        }
        else
        {
            throw new BadRequestException("Chapter does not allow it to be assigned");
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
