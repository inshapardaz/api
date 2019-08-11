using System.Collections.Generic;
using System.Linq;
using Bogus;
using Inshapardaz.Ports.Database;
using Inshapardaz.Ports.Database.Entities.Library;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Functions.Tests.DataBuilders
{
    public class CategoriesDataBuilder
    {

        private readonly IDatabaseContext _context;
        private int _bookCount;
        
        public CategoriesDataBuilder(IDatabaseContext context)
        {
            _context = context;
        }

        public CategoriesDataBuilder WithBooks(int bookCount)
        {
            _bookCount = bookCount;
            return this;
        }

        public Category Build() => Build(1).Single();
        
        public IEnumerable<Category> Build(int count)
        {
            var authorGenerator = new Faker<Author>()
                                  .RuleFor(c => c.Id, 0)
                                  .RuleFor(c => c.Name, f => f.Random.AlphaNumeric(10));

            var cats = new Faker<Category>()
                       .RuleFor(c => c.Id, 0)
                       .RuleFor(c => c.Name, f => f.Random.AlphaNumeric(10))
                       .Generate(count);

            foreach (var cat in cats)
            {
                new Faker<Book>()
                    .RuleFor(c => c.Id, 0)
                    .RuleFor(c => c.Title, f => f.Random.AlphaNumeric(10))
                    .RuleFor(c => c.Author, f => authorGenerator.Generate())
                    .Generate(_bookCount)
                    .ForEach(b => cat.BookCategories.Add(new BookCategory
                    {
                        Book = b,
                        Category = cat
                    }));
            }
            
            _context.Category.AddRange(cats);
            
            _context.SaveChanges();

            return cats;
        }

        public Category GetById(int id)
        {
            return _context.Category.SingleOrDefault(x => x.Id == id);
        }

        public IEnumerable<BookCategory> GetByBookId(int bookId)
        {
            return _context.Book
                           .Include(b => b.BookCategory)
                           .Where(b => b.Id == bookId)
                           .SelectMany(x => x.BookCategory);

        }
    }
}
