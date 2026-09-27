using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Chapter;

public class AssignChapterToUserRequest(int libraryId, int bookId, int chapterNumber, int? accountId, bool unassign = false)
    : LibraryBaseCommand(libraryId)
{
    public ChapterModel Result { get; set; }
    public int BookId { get; set; } = bookId;
    public int ChapterNumber { get; set; } = chapterNumber;
    public int? AccountId { get; private set; } = accountId;
    public bool Unassign { get; private set; } = unassign;
}

public class AssignChapterToUserRequestHandler(
    IAccountRepository accountRepository,
    IChapterRepository chapterRepository,
    IUserHelper userHelper)
    : RequestHandlerAsync<AssignChapterToUserRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AssignChapterToUserRequest> HandleAsync(AssignChapterToUserRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        if (!userHelper.IsAdmin && command.AccountId.HasValue)
        {
            var account = await accountRepository.GetLibraryAccountById(command.LibraryId, command.AccountId.Value, cancellationToken);
            if (account.Role != Role.LibraryAdmin && account.Role != Role.Writer)
            {
                throw new BadRequestException("user cannot be assigned chapter");
            }
        }

        var chapter = await chapterRepository.GetChapterById(command.LibraryId, command.BookId, command.ChapterNumber, cancellationToken);
        if (chapter == null)
        {
            throw new BadRequestException();
        }

        // No account given and not an explicit unassign means "assign to me".
        var accountId = command.Unassign ? null : command.AccountId ?? userHelper.AccountId;

        if (chapter.Status == EditingStatus.Available || chapter.Status == EditingStatus.Typing)
        {
            command.Result = await chapterRepository.UpdateWriterAssignment(command.LibraryId, command.BookId, command.ChapterNumber, accountId, cancellationToken);
        }
        else if (chapter.Status == EditingStatus.Typed || chapter.Status == EditingStatus.InReview)
        {
            command.Result = await chapterRepository.UpdateReviewerAssignment(command.LibraryId, command.BookId, command.ChapterNumber, accountId, cancellationToken);
        }
        else
        {
            throw new BadRequestException("Chapter does not allow it to be assigned");
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
