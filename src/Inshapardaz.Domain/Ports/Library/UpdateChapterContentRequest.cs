using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class UpdateChapterContentRequest : BookRequest
    {
        public UpdateChapterContentRequest(int bookId, int chapterId, string contents, string mimetype, Guid userId)
            : base(bookId, userId)
        {
            ChapterId = chapterId;
            Contents = contents;
            MimeType = mimetype;
        }

        public string MimeType { get; set; }

        public string Contents { get; set; }

        public int ChapterId { get; set; }


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
            var chapterContent = await _chapterRepository.GetChapterContent(command.BookId, command.ChapterId, command.MimeType, cancellationToken);

            if (chapterContent == null)
            {
                command.Result.ChapterContent =  await  _chapterRepository.AddChapterContent(command.BookId, 
                                                                                             command.ChapterId,
                                                                                             command.MimeType, 
                                                                                             command.Contents,
                                                                                             cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                await _chapterRepository.UpdateChapterContent(command.BookId, 
                                                              command.ChapterId,
                                                              command.MimeType, 
                                                              command.Contents,
                                                              cancellationToken);
                command.Result.ChapterContent = chapterContent;
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
