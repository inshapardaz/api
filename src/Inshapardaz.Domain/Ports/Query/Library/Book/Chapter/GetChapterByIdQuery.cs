﻿using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Query.Library.Book.Chapter;

public class GetChapterByIdQuery : LibraryBaseQuery<ChapterModel>
{
    public GetChapterByIdQuery(int libraryId, int bookId, int chapterNumber)
        : base(libraryId)
    {
        BookId = bookId;
        ChapterNumber = chapterNumber;
    }

    public int BookId { get; set; }

    public int ChapterNumber { get; }
}

public class GetChapterByIdQueryHandler : QueryHandlerAsync<GetChapterByIdQuery, ChapterModel>
{
    private readonly IChapterRepository _chapterRepository;

    public GetChapterByIdQueryHandler(IChapterRepository chapterRepository)
    {
        _chapterRepository = chapterRepository;
    }

    public override async Task<ChapterModel> ExecuteAsync(GetChapterByIdQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var chapter = await _chapterRepository.GetChapterById(command.LibraryId, command.BookId, command.ChapterNumber, cancellationToken);

        if (chapter != null)
        {
            chapter.PreviousChapter = await _chapterRepository.GetChapterById(command.LibraryId, command.BookId, command.ChapterNumber - 1, cancellationToken);
            chapter.NextChapter = await _chapterRepository.GetChapterById(command.LibraryId, command.BookId, command.ChapterNumber + 1, cancellationToken);
        }
        return chapter;
    }
}
