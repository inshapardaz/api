using System.Collections.Generic;
using System.Linq;
using Bogus;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Ports.Database;
using Inshapardaz.Ports.Database.Entities.Library;
using File = Inshapardaz.Ports.Database.Entities.File;

namespace Inshapardaz.Functions.Tests.DataBuilders
{
    public class BooksDataBuilder
    {

        private readonly IDatabaseContext _context;
        private readonly List<Book> _books = new List<Book>();
        private readonly List<File> _files = new List<File>();
        private List<Category> _categories;

        public BooksDataBuilder(IDatabaseContext context)
        {
            _context = context;
        }

        public Faker<Book> BookFaker => new Faker<Book>()
                        .RuleFor(c => c.Id, 0)
                        .RuleFor(c => c.Title, f => f.Random.AlphaNumeric(10))
                        .RuleFor(c => c.Description, f => f.Random.Words(10))
                        .RuleFor(c => c.Copyrights, f => f.PickRandom<CopyrightStatuses>())
                        .RuleFor(c => c.DateAdded, f => f.Date.Past())
                        .RuleFor(c => c.DateUpdated, f => f.Date.Past())
                        .RuleFor(c => c.ImageId, f => f.Random.Int(1))
                        .RuleFor(c => c.Language, f => f.PickRandom<Languages>())
                        .RuleFor(c => c.IsPublic, f=> f.Random.Bool())
                        .RuleFor(c => c.IsPublished, f=> f.Random.Bool())
                        .RuleFor(c => c.YearPublished, f => f.Random.Int(1900, 2000))
                        .RuleFor(c => c.Status, f => f.PickRandom<BookStatuses>());
        public BooksDataBuilder WithBooks(int count, bool havingSeries = false, int categoryCount = 0)
        {
            var books = BookFaker.Generate(count);

            var series = new Faker<Series>()
                .RuleFor(s => s.Id, 0)
                .RuleFor(s => s.Name, f => f.Random.String())
                .Generate();

            _categories = new Faker<Category>()
                          .RuleFor(c => c.Name, f => f.Random.String())
                          .Generate(categoryCount);

            int seriesIndex = 1;
            foreach (var book in books)
            {

                var author = new Faker<Author>()
                              .RuleFor(c => c.Id, 0)
                              .RuleFor(c => c.Name, f => f.Random.AlphaNumeric(10))
                              .RuleFor(c => c.ImageId, f => f.Random.Int(1))
                              .Generate();

                book.Author = author;
                if (havingSeries)
                {
                    book.Series = series;
                    book.SeriesIndex = seriesIndex++;
                }

                foreach (var category in _categories)
                {
                    book.BookCategory.Add(new BookCategory { Category = category });
                }


                if (book.ImageId.HasValue)
                {
                    _files.Add(new File {Id = book.ImageId.Value, IsPublic = true, FilePath = "http://localhost/test.jpg"});
                }
            }

            _books.AddRange(books);
                return this;
        }
        
        public IEnumerable<Book> Build()
        {
            _context.Category.AddRange(_categories);
            _context.Book.AddRange(_books);
            _context.File.AddRange(_files);
            _context.SaveChanges();

            return _books;
        }

        public Book GetById(int id)
        {
            return _context.Book.SingleOrDefault(x => x.Id == id);
        }
    }
}
