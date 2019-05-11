using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class AddChapterContentRequest : BookRequest
    {
        public AddChapterContentRequest(int bookId, int chapterId, string contents, string mimeType)
            : base(bookId)
        {
            ChapterId = chapterId;
            Contents = contents;
            MimeType = mimeType;
        }

        public int ChapterId { get; set; }

        public string Contents { get; }

        public string MimeType { get; set; }
    }

    public class AddChapterContentRequestHandler : RequestHandlerAsync<AddChapterContentRequest>
    {
        private readonly IChapterRepository _chapterRepository;

        public AddChapterContentRequestHandler(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        [BookRequestValidation(1, HandlerTiming.Before)]
        public override async Task<AddChapterContentRequest> HandleAsync(AddChapterContentRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var chapter = await _chapterRepository.GetChapterById(command.ChapterId, cancellationToken);
            if (chapter == null)
            {
                throw new BadRequestException();
            }

            await _chapterRepository.AddChapterContent(command.BookId, command.ChapterId, command.MimeType, command.Contents, cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    } 
}
