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
        private int _bookCount;
        private bool _withImage = true;

        public AuthorsDataBuilder(IDatabaseContext context, IFileStorage fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        public AuthorsDataBuilder WithBooks(int bookCount)
        {
            _bookCount = bookCount;
            return this;
        }
        
        public AuthorsDataBuilder WithoutImage()
        {
            _withImage = false;
            return this;
        }

        public Author Build()
        {
            return Build(1).Single();
        }

        public IEnumerable<Author> Build(int count)
        {
            var authors = new Faker<Author>()
                          .RuleFor(c => c.Id, 0)
                          .RuleFor(c => c.Name, f => f.Random.AlphaNumeric(10))
                          .RuleFor(c => c.ImageId, f => _withImage ? f.Random.Int(1) : (int?)null)
                          .Generate(count);
            var files = new List<File>();

            foreach (var author in authors)
            {
                var books = new Faker<Book>()
                            .RuleFor(c => c.Id, 0)
                            .RuleFor(c => c.Title, f => f.Random.AlphaNumeric(10))
                            .RuleFor(c => c.Author, author)
                            .Generate(_bookCount);

                author.Books = books;

                if (author.ImageId.HasValue)
                {
                    var url = _fileStorage.StoreFile($"{author.Id}", new Faker().Image.Random.Bytes(10), CancellationToken.None).Result;
                    files.Add(new File {Id = author.ImageId.Value, IsPublic = true, FilePath = url });

                }
            }

            _context.Author.AddRange(authors);
            _context.File.AddRange(files);
            _context.SaveChanges();

            return authors;
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
