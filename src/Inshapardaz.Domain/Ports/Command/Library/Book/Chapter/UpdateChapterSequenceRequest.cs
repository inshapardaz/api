using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Chapter;

public class UpdateChapterSequenceRequest : BookRequest
{
    public UpdateChapterSequenceRequest(int libraryId, int bookId, IEnumerable<ChapterModel> chapters)
        : base(libraryId, bookId)
    {
        Chapters = chapters;
    }

    public IEnumerable<ChapterModel> Chapters { get; }

    public IEnumerable<ChapterModel> Result { get; set; }
}

public class UpdateChapterSequenceRequestHandler : RequestHandlerAsync<UpdateChapterSequenceRequest>
{
    private readonly IChapterRepository _chapterRepository;

    public UpdateChapterSequenceRequestHandler(IChapterRepository chapterRepository)
    {
        _chapterRepository = chapterRepository;
    }

    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateChapterSequenceRequest> HandleAsync(UpdateChapterSequenceRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var chapters = await _chapterRepository.GetChaptersByBook(command.LibraryId, command.BookId, cancellationToken);

        if (chapters != null)
        {
            foreach (var c1 in command.Chapters)
            {
                var c2 = chapters.SingleOrDefault(x => x.Id == c1.Id);
                if (c2 == null)
                {
                    throw new BadRequestException("Resource out of date.");
                }
                c2.ChapterNumber = c1.ChapterNumber;
            }

            await _chapterRepository.UpdateChaptersSequence(command.LibraryId, command.BookId, chapters, cancellationToken);
            command.Result = chapters.OrderBy(c => c.ChapterNumber);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
