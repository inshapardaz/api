﻿using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Book.Chapter
{
    public class AssignChapterToUserRequest : LibraryBaseCommand
    {
        public AssignChapterToUserRequest(int libraryId, int bookId, int chapterNumber, int? accountId, bool isAdmin = false)
        : base(libraryId)
        {
            BookId = bookId;
            ChapterNumber = chapterNumber;
            AccountId = accountId;
            IsAdmin = isAdmin;
        }

        public ChapterModel Result { get; set; }
        public int BookId { get; set; }
        public int ChapterNumber { get; set; }
        public int? AccountId { get; private set; }
        public bool IsAdmin { get; }
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
            if (!command.IsAdmin)
            {
                var account = await _accountRepository.GetLibraryAccountById(command.LibraryId, command.AccountId.Value, cancellationToken);
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
                command.Result = await _chapterRepository.UpdateWriterAssignment(command.LibraryId, command.BookId, command.ChapterNumber, command.AccountId, cancellationToken);
            }
            else if (chapter.Status == EditingStatus.Typed || chapter.Status == EditingStatus.InReview)
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
