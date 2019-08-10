using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Bogus;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Ports.Database;
using Inshapardaz.Ports.Database.Entities;
using Inshapardaz.Ports.Database.Entities.Library;

namespace Inshapardaz.Functions.Tests.DataBuilders
{
    public class AuthorsDataBuilder
    {

        private readonly IDatabaseContext _context;
        private readonly IFileStorage _fileStorage;
        private readonly List<Author> _authors = new List<Author>();
        private readonly List<File> _files = new List<File>();

        public AuthorsDataBuilder(IDatabaseContext context, IFileStorage fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        public AuthorsDataBuilder WithAuthors(int count, int bookCount = 0, bool withImage = true)
        { 
            var authors = new Faker<Author>()
                .RuleFor(c => c.Id, 0)
                .RuleFor(c => c.Name, f => f.Random.AlphaNumeric(10))
                .RuleFor(c => c.ImageId, f => withImage ? f.Random.Int(1) : new int?())
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
                    var url = _fileStorage.StoreFile($"{author.Id}", new Faker().Image.Random.Bytes(10), CancellationToken.None).Result;
                    _files.Add(new File {Id = author.ImageId.Value, IsPublic = true, FilePath = url });

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

        public string GetAuthorImageUrl(int id)
        {
            var author = _context.Author.SingleOrDefault(x => x.Id == id);
            if (author?.ImageId != null)
            {
                return _context.File.SingleOrDefault(f => f.Id == author.ImageId)?.FilePath;
            }

            return null;
        }
    }
}
