using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class UpdateChapterContentRequest : BookRequest
    {
        public UpdateChapterContentRequest(ChapterContent chapter)
            : base(chapter.BookId)
        {
            Chapter = chapter;
        }

        public ChapterContent Chapter { get; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public ChapterContent ChapterContent { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateChapterContentRequestHandler : RequestHandlerAsync<UpdateChapterContentRequest>
    {
        private readonly IChapterRepository _chapterRepository;

        public UpdateChapterContentRequestHandler(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        public override async Task<UpdateChapterContentRequest> HandleAsync(UpdateChapterContentRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await _chapterRepository.GetChapterContents(command.BookId, command.Chapter.ChapterId, cancellationToken);

            if (result == null)
            {
                var chapter = command.Chapter;
                chapter.Id = default(int);
                command.Result.ChapterContent =  await  _chapterRepository.AddChapterContent(chapter, cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                await _chapterRepository.UpdateChapterContent(command.Chapter, cancellationToken);
                command.Result.ChapterContent = command.Chapter;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}