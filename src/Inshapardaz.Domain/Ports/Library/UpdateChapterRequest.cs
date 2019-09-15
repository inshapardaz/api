using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class UpdateChapterRequest : BookRequest
    {
        public UpdateChapterRequest(int bookId, int chapterId, Chapter chapter, Guid userId)
            : base(bookId, userId)
        {
            Chapter = chapter;
            Chapter.BookId = bookId;
            Chapter.Id = chapterId;
        }

        public Chapter Chapter { get; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public Chapter Chapter { get; set; }

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
            var result = await _chapterRepository.GetChapterById(command.Chapter.Id, cancellationToken);

            if (result == null)
            {
                var chapter = command.Chapter;
                chapter.Id = default(int);
                command.Result.Chapter =  await  _chapterRepository.AddChapter(command.BookId, chapter, cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                await _chapterRepository.UpdateChapter(command.Chapter, cancellationToken);
                command.Result.Chapter = command.Chapter;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
