using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Views.Library;
using Random = Inshapardaz.Functions.Tests.Helpers.Random;

namespace Inshapardaz.Functions.Tests.DataBuilders
{
    public class BooksDataBuilder
    {
        private readonly IFileStorage _fileStorage;

        private bool _hasSeries, _hasImage = true;
        private int _chapterCount, _categoriesCount, _fileCount;
        private AuthorDto _author;
        private readonly List<CategoryDto> _categories = new List<CategoryDto>();
        private SeriesDto _series;
        private int _libraryId;

        public IEnumerable<BookFileDto> Files { get; set; }

        public BooksDataBuilder(IFileStorage fileStorage)
        {
            _fileStorage = fileStorage;
        }

        public BooksDataBuilder HavingSeries()
        {
            _hasSeries = true;
            return this;
        }

        public BooksDataBuilder WithCategories(int categoriesCount)
        {
            _categoriesCount = categoriesCount;
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

        public BooksDataBuilder WithFile()
        {
            _fileCount = 1;
            return this;
        }

        public BooksDataBuilder WithFiles(int fileCount)
        {
            _fileCount = fileCount;
            return this;
        }

        internal BookView BuildView()
        {
            var fixture = new Fixture();
            return fixture.Build<BookView>()
                          .With(b => b.AuthorId, _author?.Id)
                          .With(b => b.SeriesId, _series?.Id)
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
            //var books = new Faker<BookDto>()
            //            .RuleFor(c => c.Id, 0)
            //            .RuleFor(c => c.Title, f => f.Random.AlphaNumeric(10))
            //            .RuleFor(c => c.Description, f => f.Random.Words(10))
            //            .RuleFor(c => c.Copyrights, f => f.PickRandom<CopyrightStatuses>())
            //            .RuleFor(c => c.DateAdded, f => f.Date.Past())
            //            .RuleFor(c => c.DateUpdated, f => f.Date.Past())
            //            .RuleFor(c => c.IsPublic, f => f.Random.Bool())
            //            .RuleFor(c => c.ImageId, f => _hasImage ? f.Random.Int(1) : new int?())
            //            .RuleFor(c => c.Language, f => f.PickRandom<Languages>())
            //            .RuleFor(c => c.IsPublished, f => f.Random.Bool())
            //            .RuleFor(c => c.YearPublished, f => f.Random.Int(1900, 2000))
            //            .RuleFor(c => c.Status, f => f.PickRandom<BookStatuses>())
            //            .Generate(numberOfBooks);

            //var series = _series ?? new Faker<SeriesDto>()
            //    .RuleFor(s => s.Id, 0)
            //    .RuleFor(s => s.Name, f => f.Random.String())
            //    .Generate();

            //var categories = _categories.Any()
            //    ? _categories
            //    : new Faker<CategoryDto>()
            //      .RuleFor(c => c.Name, f => f.Random.String())
            //      .Generate(_categoriesCount);

            //var files = new List<FileDto>();
            //int seriesIndex = 1;
            //foreach (var book in books)
            //{
            //    var author = _author ?? new Faker<AuthorDto>()
            //                  .RuleFor(c => c.Id, 0)
            //                  .RuleFor(c => c.Name, f => f.Random.AlphaNumeric(10))
            //                  .RuleFor(c => c.ImageId, f => f.Random.Int(1))
            //                  .Generate();

            //    book.AuthorId = author.Id;
            //    if (_hasSeries)
            //    {
            //        book.SeriesId = series.Id;
            //        book.SeriesIndex = seriesIndex++;
            //    }

            //    if (book.ImageId.HasValue)
            //    {
            //        var url = _fileStorage.StoreFile($"{book.Id}", new Faker().Image.Random.Bytes(10), CancellationToken.None).Result;
            //        files.Add(new File { Id = book.ImageId.Value, IsPublic = true, FilePath = url });
            //    }

            //    foreach (var category in categories)
            //    {
            //        book.BookCategory.Add(new BookCategory { Category = category });
            //    }

            //    for (int i = 0; i < _fileCount; i++)
            //    {
            //        var url = _fileStorage.StoreFile($"{book.Id}", new Faker().Random.Bytes(10), CancellationToken.None).Result;
            //        var file = new Faker<File>()
            //            .RuleFor(f => f.FilePath, url)
            //            .RuleFor(f => f.IsPublic, false)
            //            .RuleFor(f => f.FileName, f => f.Name.Prefix())
            //            .RuleFor(f => f.DateCreated, f => f.Date.Recent(5))
            //            .RuleFor(f => f.MimeType, "application/pdf")
            //            .Generate();
            //        book.Files.Add(new BookFile { Book = book, File = file });
            //        files.Add(file);
            //    }

            //    var chapterIndex = 1;
            //    book.Chapters = new Faker<Chapter>()
            //                                    .RuleFor(c => c.Id, 0)
            //                                    .RuleFor(c => c.Title, f => f.Random.AlphaNumeric(10))
            //                                    .RuleFor(c => c.ChapterNumber, chapterIndex++)
            //                                    .Generate(_chapterCount);
            //}

            //if (!_categories.Any())
            //{
            //    _context.Category.AddRange(categories);
            //}

            //_context.Book.AddRange(books);
            //_context.File.AddRange(files);
            //_context.SaveChanges();

            // return books;
            return null;
        }

        public void AddSomeToFavorite(IEnumerable<BookDto> books, Guid userId, int numberOfBooksToAdd)
        {
            //var favorites = new Faker().PickRandom<BookDto>(books, numberOfBooksToAdd);

            //_context.FavoriteBooks.AddRange(favorites.Select(b => new FavoriteBook
            //{
            //    BookId = b.Id,
            //    DateAdded = DateTime.Today,
            //    UserId = userId
            //}));
            //_context.SaveChanges();
        }

        public void AddBookToFavorite(BookDto book, Guid userId)
        {
            //_context.FavoriteBooks.Add(new FavoriteBook
            //{
            //    BookId = book.Id,
            //    DateAdded = DateTime.Today,
            //    UserId = userId
            //});
            //_context.SaveChanges();
        }

        public void AddSomeToRecentReads(IEnumerable<BookDto> books, Guid userId, int numberOfBooksToAdd)
        {
            //var recent = new Faker().PickRandom<Book>(books, numberOfBooksToAdd);

            //_context.RecentBooks.AddRange(recent.Select(b => new RecentBook
            //{
            //    BookId = b.Id,
            //    DateRead = DateTime.Today,
            //    UserId = userId
            //}));
            //_context.SaveChanges();
        }
    }
}
