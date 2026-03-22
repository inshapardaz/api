using AutoFixture;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Domain.Adapters.Repositories;

namespace Inshapardaz.Api.Tests.Framework.DataBuilders
{
    public class ChapterDataBuilder(
        BooksDataBuilder booksBuilder,
        IFileStorage fileStorage,
        IBookTestRepository bookRepository,
        IChapterTestRepository chapterRepository,
        IFileTestRepository fileRepository,
        IBookPageTestRepository bookPageRepository)
    {
        private readonly List<ChapterDto> _chapters = new List<ChapterDto>();
        private readonly List<ChapterContentDto> _contents = new List<ChapterContentDto>();
        private readonly List<BookPageDto> _pages = new List<BookPageDto>();
        private readonly List<FileDto> _files = new List<FileDto>();
        private readonly FakeFileStorage _fileStorage = fileStorage as FakeFileStorage;
        private int _contentCount;
        private int _libraryId;
        private bool _public;
        private string _language;
        private int? _assignedWriterId, _assignedReviewerId;
        private EditingStatus? _status;
        private bool _addPages;

        public IEnumerable<ChapterContentDto> Contents => _contents;
        public IEnumerable<ChapterDto> Chapters => _chapters;
        private IBookTestRepository _bookRepository = bookRepository;

        internal ChapterDataBuilder Public()
        {
            _public = true;
            return this;
        }

        internal ChapterDataBuilder Private()
        {
            _public = false;
            return this;
        }

        public ChapterDataBuilder WithContents(int count = 1)
        {
            _contentCount = count;
            return this;
        }

        public ChapterDataBuilder WithoutContents()
        {
            _contentCount = 0;
            return this;
        }

        internal ChapterDataBuilder WithContentLanguage(string language)
        {
            _language = language;
            return this;
        }

        internal ChapterDataBuilder WithLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        internal ChapterDataBuilder WithPages()
        {
            _addPages = true;
            return this;
        }

        internal ChapterDataBuilder WithStatus(EditingStatus status)
        {
            _status = status;
            return this;
        }

        internal ChapterDataBuilder WithoutAnyAssignment()
        {
            _assignedWriterId = _assignedReviewerId = null;
            return this;
        }

        public ChapterDto Build() => Build(1).Single();

        public IEnumerable<ChapterDto> Build(int count)
        {
            var fixture = new Fixture();
            var book = booksBuilder.WithLibrary(_libraryId).IsPublic(_public).Build();

            for (int i = 0; i < count; i++)
            {
                var chapter = fixture.Build<ChapterDto>()
                                     .With(c => c.BookId, book.Id)
                                     .With(c => c.ChapterNumber, i + 1)
                                     .With(c => c.WriterAccountId, _assignedWriterId)
                                     .With(c => c.WriterAssignTimeStamp, _assignedWriterId.HasValue ? DateTime.Now : null)
                                     .With(c => c.ReviewerAccountId, _assignedReviewerId)
                                     .With(c => c.ReviewerAssignTimeStamp, _assignedReviewerId.HasValue ? DateTime.Now : null)
                                     .With(c => c.Status, _status ?? RandomData.AssignableEditingStatus)
                                     .Create();

                chapterRepository.AddChapter(chapter);
                _chapters.Add(chapter);

                for (int j = 0; j < _contentCount; j++)
                {
                    var chapterContentFile = fixture.Build<FileDto>()
                                    .With(a => a.FilePath, RandomData.FilePath)
                                    .With(a => a.IsPublic, false)
                                    .Create();
                    fileRepository.AddFile(chapterContentFile);
                    _files.Add(chapterContentFile);

                    var chapterContentData = RandomData.Text;
                    _fileStorage.SetupFileContents(chapterContentFile.FilePath, chapterContentData);

                    var chapterContent = fixture.Build<ChapterContentDto>()
                        .With(c => c.ChapterId, chapter.Id)
                        .With(c => c.Language, _language ?? $"locale-{j}")
                        .With(c => c.FileId, chapterContentFile.Id)
                        .Create();

                    _contents.Add(chapterContent);

                    chapterRepository.AddChapterContent(chapterContent);
                }

                if (_addPages)
                {
                    var pages = fixture.Build<BookPageDto>()
                        .With(x => x.BookId, book.Id)
                        .With(x => x.ChapterId, chapter.Id)
                        .Without(x => x.WriterAccountId)
                        .Without(x => x.WriterAssignTimeStamp)
                        .Without(x => x.ReviewerAccountId)
                        .Without(x => x.ReviewerAssignTimeStamp)
                        .Without(x => x.ImageId)
                        .CreateMany(2);

                    foreach (var page in pages)
                    {
                        var bookPageContent = fixture.Build<FileDto>()
                                    .With(a => a.FilePath, RandomData.FilePath)
                                    .With(a => a.IsPublic, false)
                                    .Create();
                        fileRepository.AddFile(bookPageContent);

                        _files.Add(bookPageContent);

                        var bookPageContentData = RandomData.Text;
                        _fileStorage.SetupFileContents(bookPageContent.FilePath, bookPageContentData);
                        page.ContentId = bookPageContent.Id;
                    }
                    _pages.AddRange(pages);
                    bookPageRepository.AddBookPages(pages);
                }
            }

            return _chapters;
        }

        public void CleanUp()
        {
            fileRepository.DeleteFiles(_files);
            chapterRepository.DeleteChapterContents(_contents);
            bookPageRepository.DeleteBookPages(_pages);
            chapterRepository.DeleteChapterContents(_contents);
            chapterRepository.DeleteChapters(_chapters);
            booksBuilder.CleanUp();
        }
    }
}
