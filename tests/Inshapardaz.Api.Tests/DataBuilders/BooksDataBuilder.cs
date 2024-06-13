using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Fakes;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using RandomData = Inshapardaz.Api.Tests.Helpers.RandomData;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using System.Runtime.CompilerServices;
using Inshapardaz.Domain.Helpers;

namespace Inshapardaz.Api.Tests.DataBuilders
{

    public class BooksDataBuilder
    {
        private class AccountItemCountSpec
        {
            public int AccountId { get; set; }
            public int? Count { get; set; }
        }

        private readonly IDbConnection _connection;
        private readonly AuthorsDataBuilder _authorBuilder;
        private readonly SeriesDataBuilder _seriesBuilder;
        private readonly CategoriesDataBuilder _categoriesBuilder;

        private readonly FakeFileStorage _fileStorage;

        private List<BookDto> _books;
        private readonly List<FileDto> _files = new List<FileDto>();
        private List<AuthorDto> _authors = new List<AuthorDto>();
        private List<BookPageDto> _pages = new List<BookPageDto>();
        private List<ChapterDto> _chapters = new List<ChapterDto>();

        internal IEnumerable<BookPageDto> GetPages(int bookId) => _pages.Where(p => p.BookId == bookId);

        private bool _hasSeries, _hasImage = true;
        private bool? _isPublic = null;
        private int _chapterCount, _categoriesCount, _contentCount;
        private string _contentMimeType;

        public AuthorDto Author { get; set; }
        private List<CategoryDto> _categories = new List<CategoryDto>();
        private SeriesDto _series;
        private int _libraryId;
        private List<AccountItemCountSpec> _favoriteBooks = new List<AccountItemCountSpec>();
        private List<AccountItemCountSpec> _readBooks = new List<AccountItemCountSpec>();
        private List<BookContentDto> _contents = new List<BookContentDto>();
        private string _language = null;
        private int _numberOfAuthors;
        private List<RecentBookDto> _recentBooks = new List<RecentBookDto>();
        private int _pageCount;
        private bool _addPageImage;
        private Dictionary<int, int> _writerAssignments = new Dictionary<int, int>();
        private Dictionary<int, int> _reviewerAssignments = new Dictionary<int, int>();
        private Dictionary<EditingStatus, int> _pageStatuses = new Dictionary<EditingStatus, int>();

        public IEnumerable<AuthorDto> Authors => _authors;
        public IEnumerable<BookDto> Books => _books;
        public IEnumerable<ChapterDto> Chapters => _chapters;
        public IEnumerable<FileDto> Files => _files;
        public IEnumerable<BookContentDto> Contents => _contents;
        public IEnumerable<RecentBookDto> RecentReads => _recentBooks;

        public BooksDataBuilder(IProvideConnection connectionProvider, IFileStorage fileStorage,
                                AuthorsDataBuilder authorBuilder, SeriesDataBuilder seriesDataBuilder,
                                CategoriesDataBuilder categoriesBuilder)
        {
            _connection = connectionProvider.GetConnection();
            _fileStorage = fileStorage as FakeFileStorage;
            _authorBuilder = authorBuilder;
            _seriesBuilder = seriesDataBuilder;
            _categoriesBuilder = categoriesBuilder;
        }

        public BooksDataBuilder HavingSeries()
        {
            _hasSeries = true;
            return this;
        }

        internal BooksDataBuilder IsPublic(bool isPublic = true)
        {
            _isPublic = isPublic;
            return this;
        }

        public BooksDataBuilder WithCategories(int categoriesCount)
        {
            _categoriesCount = categoriesCount;
            return this;
        }

        public BooksDataBuilder WithCategories(IEnumerable<CategoryDto> categories)
        {
            _categories = categories.ToList();
            return this;
        }

        public BooksDataBuilder WithChapters(int chapterCount)
        {
            _chapterCount = chapterCount;
            return this;
        }

        public BooksDataBuilder WithNoImage()
        {
            _hasImage = false;
            return this;
        }

        public BooksDataBuilder WithAuthor(AuthorDto author)
        {
            Author = author;
            return this;
        }

        public BooksDataBuilder WithAuthors(IEnumerable<AuthorDto> authors)
        {
            _authors = authors.ToList();
            return this;
        }

        public BooksDataBuilder WithAuthors(int numberOfAuthors)
        {
            _numberOfAuthors = numberOfAuthors;
            return this;
        }

        public BooksDataBuilder WithCategory(CategoryDto category)
        {
            _categories.Add(category);
            return this;
        }

        internal BooksDataBuilder WithLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public BooksDataBuilder WithSeries(SeriesDto series)
        {
            _series = series;
            return this;
        }

        public BooksDataBuilder WithContent()
        {
            _contentCount = 1;
            return this;
        }

        public BooksDataBuilder WithContents(int contentCount, string mimeType = null)
        {
            _contentCount = contentCount;
            _contentMimeType = mimeType;
            return this;
        }

        public BooksDataBuilder WithPages(int count = 10, bool addImage = false)
        {
            _pageCount = count;
            _addPageImage = addImage;
            return this;
        }

        public BooksDataBuilder AssignPagesToWriter(int accountId, int count)
        {
            _writerAssignments.TryAdd(accountId, count);
            return this;
        }

        public BooksDataBuilder AssignPagesToReviewer(int accountId, int count)
        {
            _reviewerAssignments.TryAdd(accountId, count);
            return this;
        }

        public BooksDataBuilder WithStatus(EditingStatus statuses, int count)
        {
            _pageStatuses.TryAdd(statuses, count);
            return this;
        }

        internal BooksDataBuilder WithContentLanguage(string language)
        {
            _language = language;
            return this;
        }

        public BooksDataBuilder AddToFavorites(int accountId, int? countOfbookToAddToFavorite = null)
        {
            _favoriteBooks.Add(new AccountItemCountSpec() { AccountId = accountId, Count = countOfbookToAddToFavorite });
            return this;
        }

        public BooksDataBuilder AddToRecentReads(int accountId, int? countOfBookToAddToRecent = null)
        {
            _readBooks.Add(new AccountItemCountSpec() { AccountId = accountId, Count = countOfBookToAddToRecent });

            return this;
        }

        internal BookView BuildView()
        {
            var fixture = new Fixture();

            if (Author == null)
            {
                Author = _authorBuilder.WithLibrary(_libraryId).Build(1).Single();
            }

            SeriesDto series = _series;
            if (_hasSeries && _series == null)
            {
                series = _seriesBuilder.WithLibrary(_libraryId).Build();
            }

            return fixture.Build<BookView>()
                          .With(b => b.Authors, new List<AuthorView> { new AuthorView { Id = Author.Id } })
                          .With(b => b.SeriesId, series.Id)
                          .With(b => b.SeriesIndex, RandomData.Number)
                          .With(b => b.Categories, _categories.Any() ? _categories.Select(c => c.ToView()) : new CategoryView[0])
                          .With(b => b.Language, RandomData.Locale)
                          .Create();
        }

        public BookDto Build()
        {
            return Build(1).Single();
        }

        public IEnumerable<BookDto> Build(int numberOfBooks)
        {
            var fixture = new Fixture();

            if (Author == null && !_authors.Any())
            {
                _authors = _authorBuilder.WithLibrary(_libraryId).Build(_numberOfAuthors > 0 ? _numberOfAuthors : numberOfBooks).ToList();
            }

            Func<bool> isPublic = () => _isPublic ?? RandomData.Bool;

            _books = fixture.Build<BookDto>()
                          .With(b => b.LibraryId, _libraryId)
                          .With(b => b.ImageId, _hasImage ? RandomData.Number : 0)
                          .With(b => b.IsPublic, isPublic)
                          .With(b => b.Language, RandomData.Locale)
                          .With(b => b.SeriesIndex, _hasSeries ? RandomData.Number : (int?)null)
                          .With(b => b.DateAdded, RandomData.Date)
                          .With(b => b.DateUpdated, RandomData.Date)
                          .With(b => b.Status, Domain.Models.StatusType.Published)
                          .CreateMany(numberOfBooks)
                          .ToList();

            IEnumerable<CategoryDto> categories;

            if (_categoriesCount > 0 && !_categories.Any())
            {
                categories = _categoriesBuilder.WithLibrary(_libraryId).Build(_categoriesCount);
            }
            else
            {
                categories = _categories;
            }

            foreach (var book in _books)
            {
                if (_hasSeries && _series == null)
                {
                    var series = _seriesBuilder.WithLibrary(_libraryId).Build();
                    book.SeriesId = series.Id;
                }
                else
                {
                    book.SeriesId = _series?.Id;
                }

                FileDto bookImage = null;
                if (_hasImage)
                {
                    bookImage = fixture.Build<FileDto>()
                                         .With(a => a.FilePath, RandomData.FilePath)
                                         .With(a => a.IsPublic, true)
                                         .Create();
                    _connection.AddFile(bookImage);

                    _files.Add(bookImage);
                    _fileStorage.SetupFileContents(bookImage.FilePath, RandomData.Bytes);
                    _connection.AddFile(bookImage);

                    book.ImageId = bookImage.Id;
                }
                else
                {
                    book.ImageId = null;
                }

                List<FileDto> files = null;
                if (_contentCount > 0)
                {
                    var mimeType = _contentMimeType ?? RandomData.MimeType;
                    var filePath = FilePathHelper.GetBookChapterContentPath(_libraryId, book.Id, FilePathHelper.GetBookContentFileName(RandomData.FileName(mimeType)));
                    files = fixture.Build<FileDto>()
                                         .With(f => f.FilePath, filePath)
                                         .With(f => f.IsPublic, false)
                                         .With(f => f.MimeType, mimeType)
                                         .CreateMany(_contentCount)
                                         .ToList();
                    _files.AddRange(files);
                    files.ForEach(f => _fileStorage.SetupFileContents(f.FilePath, RandomData.Bytes));
                    _connection.AddFiles(files);
                }

                _connection.AddBook(book);

                if (Author != null)
                {
                    _connection.AddBookAuthor(book.Id, Author.Id);
                }
                else
                {
                    foreach (var author in _authors)
                    {
                        _connection.AddBookAuthor(book.Id, author.Id);
                    }
                }

                if (categories != null && categories.Any())
                    _connection.AddBookToCategories(book.Id, categories);

                if (files != null)
                {
                    var contents = files.Select(f => new BookContentDto
                    {
                        BookId = book.Id,
                        Language = _language ?? RandomData.NextLocale(),
                        FileId = f.Id,
                        MimeType = f.MimeType,
                        FilePath = f.FilePath
                    }).ToList();
                    _connection.AddBookFiles(book.Id, contents);
                    _contents.AddRange(contents);
                }

                if (_chapterCount > 0)
                {
                    int c = 1;
                    var chapters = fixture.Build<ChapterDto>()
                        .With(c => c.BookId, book.Id)
                        .With(c => c.ChapterNumber, () => c++)
                        .Without(c => c.WriterAccountId)
                        .Without(c => c.WriterAssignTimeStamp)
                        .Without(c => c.ReviewerAccountId)
                        .Without(c => c.ReviewerAssignTimeStamp)
                        .CreateMany(_chapterCount);

                    _connection.AddChapters(chapters);
                    _chapters.AddRange(chapters);
                }
                if (_pageCount > 0)
                {
                    var pages = new List<BookPageDto>();

                    for (int i = 0; i < _pageCount; i++)
                    {
                        FileDto pageImage = null;
                        if (_addPageImage)
                        {
                            pageImage = fixture.Build<FileDto>()
                                         .With(a => a.FilePath, RandomData.FilePath)
                                         .With(a => a.IsPublic, true)
                                         .Create();
                            _connection.AddFile(pageImage);

                            _files.Add(pageImage);
                            _fileStorage.SetupFileContents(pageImage.FilePath, RandomData.Bytes);
                            _connection.AddFile(pageImage);
                        }

                        var bookPageContent = fixture.Build<FileDto>()
                                    .With(a => a.FilePath, RandomData.FilePath)
                                    .With(a => a.IsPublic, false)
                                    .Create();
                        _connection.AddFile(bookPageContent);

                        _files.Add(bookPageContent);

                        var bookPageContentData = RandomData.Text;
                        _fileStorage.SetupFileContents(bookPageContent.FilePath, bookPageContentData);
                        
                        var bookPage = fixture.Build<BookPageDto>()
                            .With(p => p.BookId, book.Id)
                            .With(p => p.SequenceNumber, i + 1)
                            .With(p => p.ImageId, pageImage?.Id)
                            .With(p => p.ContentId, bookPageContent?.Id)
                            .With(p => p.WriterAccountId, (int?)null)
                            .With(p => p.ReviewerAccountId, (int?)null)
                            .With(p => p.Status, EditingStatus.All)
                            .With(p => p.Text, bookPageContentData)
                            .Create();
                        
                        pages.Add(bookPage);
                    }

                    if (_writerAssignments.Any())
                    {
                        foreach (var assignment in _writerAssignments)
                        {
                            var pagesToAssign = RandomData.PickRandom(pages.Where(p => p.WriterAccountId == null && p.ReviewerAccountId == null), assignment.Value);
                            foreach (var pageToAssign in pagesToAssign)
                            {
                                pageToAssign.WriterAccountId = assignment.Key;
                            }
                        }
                    }

                    if (_reviewerAssignments.Any())
                    {
                        foreach (var assignment in _reviewerAssignments)
                        {
                            var pagesToAssign = RandomData.PickRandom(pages.Where(p => p.WriterAccountId == null && p.ReviewerAccountId == null), assignment.Value);
                            foreach (var pageToAssign in pagesToAssign)
                            {
                                pageToAssign.ReviewerAccountId = assignment.Key;
                            }
                        }
                    }

                    if (_pageStatuses.Any())
                    {
                        foreach (var pageStatus in _pageStatuses)
                        {
                            var pagesToSetStatus = RandomData.PickRandom(pages.Where(p => p.Status == EditingStatus.All), pageStatus.Value);
                            foreach (var pageToSetStatus in pagesToSetStatus)
                            {
                                pageToSetStatus.Status = pageStatus.Key;
                            }
                        }
                    }

                    _connection.AddBookPages(pages);
                    _pages.AddRange(pages);
                }
            }

            if (_favoriteBooks.Any())
            {
                foreach (var f in _favoriteBooks)
                {
                    var booksToAddToFavorite = f.Count.HasValue ? _books.PickRandom(f.Count.Value) : _books;
                    if (f.AccountId != 0)
                        _connection.AddBooksToFavorites(_libraryId, booksToAddToFavorite.Select(b => b.Id), f.AccountId);
                }
            }

            if (_readBooks.Any())
            {
                foreach (var r in _readBooks)
                {
                    var booksToAddToRecent = r.Count.HasValue ? _books.PickRandom(r.Count.Value) : _books;
                    foreach (var recentBook in booksToAddToRecent)
                    {
                        if (r.AccountId != 0)
                        {
                            RecentBookDto recent = new RecentBookDto { LibraryId = _libraryId, BookId = recentBook.Id, AccountId = r.AccountId, DateRead = RandomData.Date };
                            _connection.AddBookToRecentReads(recent);
                            _recentBooks.Add(recent);
                        }
                    }
                }
            }

            return _books;
        }

        public void CleanUp()
        {
            _connection.DeleteBooks(_books);
            _connection.DeleteFiles(_files);
            _connection.DeleteBookPages(_pages);
            _connection.DeleteChapters(_chapters);
            _seriesBuilder.CleanUp();
            _authorBuilder.CleanUp();
        }
    }
}
