using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Bogus;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Ports.Database;
using Inshapardaz.Ports.Database.Entities.Library;
using File = Inshapardaz.Ports.Database.Entities.File;

namespace Inshapardaz.Functions.Tests.DataBuilders
{
    public class BooksDataBuilder
    {

        private readonly IDatabaseContext _context;
        private readonly IFileStorage _fileStorage;

        private bool _hasSeries, _hasImage = true;
        private int _chapterCount, _categoriesCount, _fileCount;
        private Author _author;
        private readonly List<Category> _categories = new List<Category>();
        private Series _series;

        public BooksDataBuilder(IDatabaseContext context, IFileStorage fileStorage)
        {
            _context = context;
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
        
        public BooksDataBuilder WithAuthor(Author author)
        {
            _author = author;
            return this;
        }
        
        public BooksDataBuilder WithCategory(Category category)
        {
            _categories.Add(category);
            return this;
        }

        public BooksDataBuilder WithSeries(Series series)
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


        public Book Build()
        {
            return Build(1).Single();
        }

        public IEnumerable<Book> Build(int numberOfBooks)
        {
            var books = new Faker<Book>()
                        .RuleFor(c => c.Id, 0)
                        .RuleFor(c => c.Title, f => f.Random.AlphaNumeric(10))
                        .RuleFor(c => c.Description, f => f.Random.Words(10))
                        .RuleFor(c => c.Copyrights, f => f.PickRandom<CopyrightStatuses>())
                        .RuleFor(c => c.DateAdded, f => f.Date.Past())
                        .RuleFor(c => c.DateUpdated, f => f.Date.Past())
                        .RuleFor(c => c.IsPublic, f => f.Random.Bool())
                        .RuleFor(c => c.ImageId, f => _hasImage ? f.Random.Int(1) : new int?())
                        .RuleFor(c => c.Language, f => f.PickRandom<Languages>())
                        .RuleFor(c => c.IsPublished, f => f.Random.Bool())
                        .RuleFor(c => c.YearPublished, f => f.Random.Int(1900, 2000))
                        .RuleFor(c => c.Status, f => f.PickRandom<BookStatuses>())
                        .RuleFor(c => c.Files, new List<BookFile>())
                        .Generate(numberOfBooks);

            var series = _series ?? new Faker<Series>()
                .RuleFor(s => s.Id, 0)
                .RuleFor(s => s.Name, f => f.Random.String())
                .Generate();

            var categories = _categories.Any()
                ? _categories
                : new Faker<Category>()
                  .RuleFor(c => c.Name, f => f.Random.String())
                  .Generate(_categoriesCount);

            var files = new List<File>();
            int seriesIndex = 1;
            foreach (var book in books)
            {

                var author = _author ?? new Faker<Author>()
                              .RuleFor(c => c.Id, 0)
                              .RuleFor(c => c.Name, f => f.Random.AlphaNumeric(10))
                              .RuleFor(c => c.ImageId, f => f.Random.Int(1))
                              .Generate();

                book.Author = author;
                if (_hasSeries)
                {
                    book.Series = series;
                    book.SeriesIndex = seriesIndex++;
                }

                if (book.ImageId.HasValue)
                {
                    var url = _fileStorage.StoreFile($"{book.Id}", new Faker().Image.Random.Bytes(10), CancellationToken.None).Result;
                    files.Add(new File { Id = book.ImageId.Value, IsPublic = true, FilePath = url });
                }

                foreach (var category in categories)
                {
                    book.BookCategory.Add(new BookCategory { Category = category });
                }

                for (int i = 0; i < _fileCount; i++)
                {
                    var url = _fileStorage.StoreFile($"{book.Id}", new Faker().Random.Bytes(10), CancellationToken.None).Result;
                    var file = new Faker<File>()
                        .RuleFor(f => f.FilePath, url)
                        .RuleFor(f => f.IsPublic, false)
                        .RuleFor(f => f.FileName, f => f.Name.Prefix())
                        .RuleFor(f => f.DateCreated, f => f.Date.Recent(5))
                        .RuleFor(f => f.MimeType, "application/pdf")
                        .Generate();
                    book.Files.Add(new BookFile { Book = book, File = file });
                    files.Add(file);
                }
                
                var chapterIndex = 1;
                book.Chapters = new Faker<Chapter>()
                                                .RuleFor(c => c.Id, 0)
                                                .RuleFor(c => c.Title, f => f.Random.AlphaNumeric(10))
                                                .RuleFor(c => c.ChapterNumber, chapterIndex++)
                                                .Generate(_chapterCount);
            }

            if (!_categories.Any())
            {
                _context.Category.AddRange(categories);
            }
            
            _context.Book.AddRange(books);
            _context.File.AddRange(files);
            _context.SaveChanges();

            return books;
        }

        public void AddSomeToFavorite(IEnumerable<Book> books, Guid userId, int numberOfBooksToAdd)
        {
            var favorites = new Faker().PickRandom<Book>(books, numberOfBooksToAdd);
            
            _context.FavoriteBooks.AddRange(favorites.Select(b => new FavoriteBook
            {
                BookId = b.Id,
                DateAdded = DateTime.Today,
                UserId = userId
            }));
            _context.SaveChanges();
        }

        public void AddBookToFavorite(Book book, Guid userId)
        {
            _context.FavoriteBooks.Add(new FavoriteBook
            {
                BookId = book.Id,
                DateAdded = DateTime.Today,
                UserId = userId
            });
            _context.SaveChanges();
        }

        public void AddSomeToRecentReads(IEnumerable<Book> books, Guid userId, int numberOfBooksToAdd)
        {
            var recent = new Faker().PickRandom<Book>(books, numberOfBooksToAdd);

            _context.RecentBooks.AddRange(recent.Select(b => new RecentBook
            {
                BookId = b.Id,
                DateRead= DateTime.Today,
                UserId = userId
            }));
            _context.SaveChanges();
        }

        public void AddBookToRecentReads(Book book, Guid userId)
        {
            _context.RecentBooks.Add(new RecentBook
            {
                BookId = book.Id,
                DateRead = DateTime.Today,
                UserId = userId
            });
            _context.SaveChanges();
        }

        public Book GetById(int id)
        {
            return _context.Book.SingleOrDefault(x => x.Id == id);
        }

        public string GetBookImageUrl(int id)
        {
            var book = _context.Book.SingleOrDefault(x => x.Id == id);
            if (book?.ImageId != null)
            {
                return _context.File.SingleOrDefault(f => f.Id == book.ImageId)?.FilePath;
            }

            return null;
        }

        public List<RecentBook> GetRecentBooks()
        {
            return _context.RecentBooks.ToList();
        }

        public bool DoesBookExistsInFavorites(Book book) =>
            _context.FavoriteBooks.Any(x => x.BookId == book.Id);

        public bool DoesBookExistsInRecent(Book book) =>
            _context.RecentBooks.Any(x => x.BookId == book.Id);

        public List<BookFile> GetBookFiles(int bookId)
        {
            return _context.BookFiles.Where(bf => bf.BookId == bookId).ToList();
        }

        public File GetFileById(int fileId)
        {
            return _context.File.SingleOrDefault(f => f.Id == fileId);
        }
    }
}
