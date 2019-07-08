using System.Collections.Generic;
using System.Linq;
using Bogus;
using Inshapardaz.Ports.Database;
using Inshapardaz.Ports.Database.Entities;
using Inshapardaz.Ports.Database.Entities.Library;

namespace Inshapardaz.Functions.Tests.DataBuilders
{
    public class AuthorsDataBuilder
    {

        private readonly IDatabaseContext _context;
        private readonly List<Author> _authors = new List<Author>();
        private readonly List<File> _files = new List<File>();

        public AuthorsDataBuilder(IDatabaseContext context)
        {
            _context = context;
        }

        public AuthorsDataBuilder WithAuthors(int count, int bookCount = 0)
        { 
            var authors = new Faker<Author>()
                .RuleFor(c => c.Id, 0)
                .RuleFor(c => c.Name, f => f.Random.AlphaNumeric(10))
                .RuleFor(c => c.ImageId, f => f.Random.Int(1))
                .Generate(count);

            foreach (var author in authors)
            {
                var books = new Faker<Book>()
                            .RuleFor(c => c.Id, 0)
                            .RuleFor(c => c.Title, f => f.Random.AlphaNumeric(10))
                            .RuleFor(c => c.Author, author)
                            .Generate(bookCount);

                author.Books = books;

                if (author.ImageId.HasValue)
                {
                    _files.Add(new File {Id = author.ImageId.Value, IsPublic = true, FilePath = "http://localhost/test.jpg"});

                }
            }

            _authors.AddRange(authors);
                return this;
        }
        
        public IEnumerable<Author> Build()
        {
            _context.Author.AddRange(_authors);
            _context.File.AddRange(_files);
            _context.SaveChanges();

            return _authors;
        }

        public Author GetById(int id)
        {
            return _context.Author.SingleOrDefault(x => x.Id == id);
        }
    }
}
