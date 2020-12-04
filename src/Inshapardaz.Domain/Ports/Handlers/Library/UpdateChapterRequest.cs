﻿using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Models.Library
{
    public class UpdateChapterRequest : BookRequest
    {
        public UpdateChapterRequest(int libraryId, int bookId, int chapterId, ChapterModel chapter)
            : base(libraryId, bookId)
        {
            Chapter = chapter;
            Chapter.BookId = bookId;
            Chapter.Id = chapterId;
        }

        public ChapterModel Chapter { get; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public ChapterModel Chapter { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateChapterRequestHandler : RequestHandlerAsync<UpdateChapterRequest>
    {
        private readonly IChapterRepository _chapterRepository;

        public UpdateChapterRequestHandler(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        public override async Task<UpdateChapterRequest> HandleAsync(UpdateChapterRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _chapterRepository.GetChapterById(command.LibraryId, command.BookId, command.Chapter.Id, cancellationToken);

            if (result == null)
            {
                var chapter = command.Chapter;
                chapter.Id = default(int);
                command.Result.Chapter = await _chapterRepository.AddChapter(command.LibraryId, command.BookId, chapter, cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                await _chapterRepository.UpdateChapter(command.LibraryId, command.BookId, command.Chapter, cancellationToken);
                command.Result.Chapter = command.Chapter;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
