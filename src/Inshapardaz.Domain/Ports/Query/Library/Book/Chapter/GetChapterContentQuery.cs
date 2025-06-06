﻿using Inshapardaz.Domain.Exception;
using Paramore.Darker;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;

namespace Inshapardaz.Domain.Ports.Query.Library.Book.Chapter;

public class GetChapterContentQuery : LibraryBaseQuery<ChapterContentModel>
{
    public GetChapterContentQuery(int libraryId, int bookId, int chapterNumber, string language)
        : base(libraryId)
    {
        BookId = bookId;
        ChapterNumber = chapterNumber;
        Language = language;
    }

    public int BookId { get; set; }

    public int ChapterNumber { get; }

    public string Language { get; set; }
}

public class GetChapterContentQueryHandler : QueryHandlerAsync<GetChapterContentQuery, ChapterContentModel>
{
    private readonly ILibraryRepository _libraryRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IChapterRepository _chapterRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorage _fileStorage;
    private readonly IUserHelper _userHelper;

    public GetChapterContentQueryHandler(ILibraryRepository libraryRepository, IBookRepository bookRepository, IChapterRepository chapterRepository, IUserHelper userHelper, IFileRepository fileRepository, IFileStorage fileStorage)
    {
        _libraryRepository = libraryRepository;
        _bookRepository = bookRepository;
        _chapterRepository = chapterRepository;
        _userHelper = userHelper;
        _fileRepository = fileRepository;
        _fileStorage = fileStorage;
    }

    [LibraryAuthorize(1)]
    public override async Task<ChapterContentModel> ExecuteAsync(GetChapterContentQuery command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await _bookRepository.GetBookById(command.LibraryId, command.BookId, null, cancellationToken);
        if (book == null)
        {
            throw new NotFoundException();
        }

        if (!book.IsPublic && !_userHelper.AccountId.HasValue)
        {
            throw new UnauthorizedException();
        }

        if (string.IsNullOrWhiteSpace(command.Language))
        {
            var library = await _libraryRepository.GetLibraryById(command.LibraryId, cancellationToken);
            if (library == null)
            {
                throw new BadRequestException();
            }

            command.Language = library.Language;
        }

        var chapterContent = await _chapterRepository.GetChapterContent(command.LibraryId, command.BookId, command.ChapterNumber, command.Language, cancellationToken);
        if (chapterContent != null)
        {
            if (chapterContent.FileId.HasValue)
            {
                var file = await _fileRepository.GetFileById(chapterContent.FileId.Value, cancellationToken);
                if (file != null)
                {
                    var fc = await _fileStorage.GetTextFile(file.FilePath, cancellationToken);
                    chapterContent.Text = fc;
                }
            }
            if (_userHelper.AccountId.HasValue)
            {
                await _bookRepository.AddRecentBook(
                    command.LibraryId, 
                    _userHelper.AccountId.Value, 
                    command.BookId, new ReadProgressModel()
                    {
                        ProgressType = ProgressType.Chapter,
                        ProgressId = command.ChapterNumber,
                        ProgressValue = 0.0
                    }, 
                    cancellationToken);
            }
        }

        return chapterContent;
    }
}
