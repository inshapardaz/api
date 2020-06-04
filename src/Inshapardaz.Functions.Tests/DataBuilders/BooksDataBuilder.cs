using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using AutoFixture;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Inshapardaz.Ports.Database;
using Random = Inshapardaz.Functions.Tests.Helpers.Random;

namespace Inshapardaz.Functions.Tests.DataBuilders
{
    public class BooksDataBuilder
    {
        private readonly IDbConnection _connection;
        private readonly AuthorsDataBuilder _authorBuilder;
        private readonly SeriesDataBuilder _seriesBuilder;
        private readonly CategoriesDataBuilder _categoriesBuilder;
        private readonly FakeFileStorage _fileStorage;

        private List<BookDto> _books;
        private readonly List<FileDto> _files = new List<FileDto>();

        private bool _hasSeries, _hasImage = true, _isPublic = Random.Bool;
        private int _chapterCount, _categoriesCount, _contentCount;

        private AuthorDto _author;
        private List<CategoryDto> _categories = new List<CategoryDto>();
        private SeriesDto _series;
        private int _libraryId;
        private int? _bookCountToAddToFavorite;
        private Guid _addToFavoriteForUser;
        private int? _bookCountToAddToRecent;
        private Guid _addToRecentReadsForUser;
        private List<BookContentDto> _contents = new List<BookContentDto>();

        public IEnumerable<BookContentDto> Contents => _contents;

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
            _author = author;
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

        public BooksDataBuilder WithContents(int contentCount)
        {
            _contentCount = contentCount;
            return this;
        }

        public BooksDataBuilder AddToFavorites(Guid userId, int? bookToAddToFavorite = null)
        {
            _bookCountToAddToFavorite = bookToAddToFavorite;
            _addToFavoriteForUser = userId;
            return this;
        }

        public BooksDataBuilder AddToRecentReads(Guid userId, int? bookToAddToRecent = null)
        {
            _bookCountToAddToRecent = bookToAddToRecent;
            _addToRecentReadsForUser = userId;
            return this;
        }

        internal BookView BuildView()
        {
            var fixture = new Fixture();

            if (_author == null)
            {
                _author = _authorBuilder.WithLibrary(_libraryId).Build(1).Single();
            }

            SeriesDto series = _series;
            if (_hasSeries && _series == null)
            {
                series = _seriesBuilder.WithLibrary(_libraryId).Build();
            }

            return fixture.Build<BookView>()
                          .With(b => b.AuthorId, _author.Id)
                          .With(b => b.SeriesId, series.Id)
                          .With(b => b.SeriesIndex, Random.Number)
                          .With(b => b.Categories, _categories.Any() ? _categories.Select(c => c.ToView()) : new CategoryView[0])
                          .Create();
        }

        public BookDto Build()
        {
            return Build(1).Single();
        }

        public IEnumerable<BookDto> Build(int numberOfBooks)
        {
            var fixture = new Fixture();

            IEnumerable<AuthorDto> authors = null;
            if (_author == null)
            {
                authors = _authorBuilder.WithLibrary(_libraryId).Build(numberOfBooks);
            }

            _books = fixture.Build<BookDto>()
                          .With(b => b.LibraryId, _libraryId)
                          .With(b => b.AuthorId, _author != null ? _author.Id : Random.PickRandom(authors).Id)
                          .With(b => b.ImageId, _hasImage ? Random.Number : 0)
                          .With(b => b.IsPublic, _isPublic)
                          .With(b => b.SeriesIndex, _hasSeries ? Random.Number : (int?)null)
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
                                         .With(a => a.FilePath, Random.BlobUrl)
                                         .With(a => a.IsPublic, true)
                                         .Create();
                    _connection.AddFile(bookImage);

                    _files.Add(bookImage);
                    _fileStorage.SetupFileContents(bookImage.FilePath, Random.Bytes);
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
                    var mimeType = Random.MimeType;
                    files = fixture.Build<FileDto>()
                                         .With(f => f.FilePath, Random.BlobUrl)
                                         .With(f => f.IsPublic, false)
                                         .With(f => f.MimeType, mimeType)
                                         .With(f => f.FilePath, Random.BlobUrl)
                                         .CreateMany(_contentCount)
                                         .ToList();
                    _files.AddRange(files);
                    files.ForEach(f => _fileStorage.SetupFileContents(f.FilePath, Random.Bytes));
                    _connection.AddFiles(files);
                }

                _connection.AddBook(book);

                if (categories != null && categories.Any())
                    _connection.AddBookToCategories(book.Id, categories);

                if (files != null)
                {
                    var contents = files.Select(f => new BookContentDto
                    {
                        BookId = book.Id,
                        Language = Random.NextLocale(),
                        FileId = f.Id,
                        MimeType = f.MimeType,
                        FilePath = f.FilePath
                    }).ToList();
                    _connection.AddBookFiles(book.Id, contents);
                    _contents.AddRange(contents);
                }
            }

            if (_addToFavoriteForUser != Guid.Empty)
            {
                var booksToAddToFavorite = _bookCountToAddToFavorite.HasValue ? _books.PickRandom(_bookCountToAddToFavorite.Value) : _books;
                _connection.AddBooksToFavorites(_libraryId, booksToAddToFavorite.Select(b => b.Id), _addToFavoriteForUser);
            }

            if (_addToRecentReadsForUser != Guid.Empty)
            {
                var booksToAddToRecent = _bookCountToAddToRecent.HasValue ? _books.PickRandom(_bookCountToAddToRecent.Value) : _books;
                _connection.AddBooksToRecentReads(_libraryId, booksToAddToRecent.Select(b => b.Id), _addToRecentReadsForUser);
            }

            return _books;
        }

        public void CleanUp()
        {
            _connection.DeleteBooks(_books);
            _connection.DeleteFiles(_files);
            _seriesBuilder.CleanUp();
            _authorBuilder.CleanUp();
        }

        //public void AddSomeToFavorite(IEnumerable<BookDto> books, Guid userId, int numberOfBooksToAdd)
        //{
        //    //var favorites = new Faker().PickRandom<BookDto>(books, numberOfBooksToAdd);

        //    //_context.FavoriteBooks.AddRange(favorites.Select(b => new FavoriteBook
        //    //{
        //    //    BookId = b.Id,
        //    //    DateAdded = DateTime.Today,
        //    //    UserId = userId
        //    //}));
        //    //_context.SaveChanges();
        //}

        //public void AddBookToFavorite(BookDto book, Guid userId)
        //{
        //    //_context.FavoriteBooks.Add(new FavoriteBook
        //    //{
        //    //    BookId = book.Id,
        //    //    DateAdded = DateTime.Today,
        //    //    UserId = userId
        //    //});
        //    //_context.SaveChanges();
        //}

        //public void AddSomeToRecentReads(IEnumerable<BookDto> books, Guid userId, int numberOfBooksToAdd)
        //{
        //    //var recent = new Faker().PickRandom<Book>(books, numberOfBooksToAdd);

        //    //_context.RecentBooks.AddRange(recent.Select(b => new RecentBook
        //    //{
        //    //    BookId = b.Id,
        //    //    DateRead = DateTime.Today,
        //    //    UserId = userId
        //    //}));
        //    //_context.SaveChanges();
        //}
    }
}
