using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book.Chapter;

public class UpdateChapterSequenceRequest(int libraryId, int bookId, IEnumerable<ChapterModel> chapters)
    : BookRequest(libraryId, bookId)
{
    public IEnumerable<ChapterModel> Chapters { get; } = chapters;

    public IEnumerable<ChapterModel> Result { get; set; }
}

public class UpdateChapterSequenceRequestHandler(IChapterRepository chapterRepository)
    : RequestHandlerAsync<UpdateChapterSequenceRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateChapterSequenceRequest> HandleAsync(UpdateChapterSequenceRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var chapters = await chapterRepository.GetChaptersByBook(command.LibraryId, command.BookId, cancellationToken);

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

            await chapterRepository.UpdateChaptersSequence(command.LibraryId, command.BookId, chapters, cancellationToken);
            command.Result = chapters.OrderBy(c => c.ChapterNumber);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
