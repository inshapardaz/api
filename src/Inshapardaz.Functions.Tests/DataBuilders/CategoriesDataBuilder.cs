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
        private readonly List<Category> _categories = new List<Category>();

        public CategoriesDataBuilder(IDatabaseContext context)
        {
            _context = context;
        }

        public CategoriesDataBuilder WithCategories(int count, int bookCount = 0)
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
                    .Generate(bookCount)
                    .ForEach(b => cat.BookCategories.Add(new BookCategory
                {
                    Book = b,
                    Category = cat
                }));
            }
            
            _categories.AddRange(cats);
            return this;
        }
        
        public IEnumerable<Category> Build()
        {
            _context.Category.AddRange(_categories);
            
            _context.SaveChanges();

            return _categories;
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
