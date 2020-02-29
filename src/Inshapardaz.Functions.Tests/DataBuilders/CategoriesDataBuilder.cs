using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using Bogus;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Ports.Database;
using Inshapardaz.Ports.Database.Entities.Library;

namespace Inshapardaz.Functions.Tests.DataBuilders
{
    public class CategoriesDataBuilder
    {
        private readonly IDatabaseContext _context;
        private readonly IDbConnection _connection;
        private int _bookCount;
        private List<AuthorDto> _authors = new List<AuthorDto>();
        private List<CategoryDto> _categories = new List<CategoryDto>();
        public LibraryDto Library { get; private set; }

        public CategoriesDataBuilder(IDatabaseContext context, IProvideConnection connectionProvider)
        {
            _context = context;
            _connection = connectionProvider.GetConnection();
        }

        public CategoriesDataBuilder WithBooks(int bookCount)
        {
            _bookCount = bookCount;
            return this;
        }

        public CategoryDto Build() => Build(1).Single();

        public IEnumerable<CategoryDto> Build(int count)
        {
            var fixture = new Fixture();
            Library = fixture.Build<LibraryDto>()
                                 .With(l => l.Language, "en")
                                 .Create();

            _connection.AddLibrary(Library);

            var cats = fixture.Build<CategoryDto>()
                              .With(c => c.LibraryId, Library.Id)
                               .CreateMany(count);

            _connection.AddCategories(cats);
            _categories.AddRange(cats);

            foreach (var cat in cats)
            {
                var author = fixture.Build<AuthorDto>()
                                     .With(a => a.LibraryId, Library.Id)
                                     .Without(a => a.ImageId)
                                     .Create();

                _connection.AddAuthor(author);
                _authors.Add(author);

                var books = fixture.Build<BookDto>()
                                   .With(b => b.AuthorId, author.Id)
                                   .With(b => b.LibraryId, Library.Id)
                                   .Without(b => b.ImageId)
                                   .Without(b => b.SeriesId)
                                   .CreateMany(_bookCount);
                _connection.AddBooks(books);

                _connection.AddBooksToCategory(books, cat);
            }

            return cats;
        }

        public void CleanUp()
        {
            if (Library != null)
                _connection.DeleteLibrary(Library.Id);

            _connection.DeleteAuthors(_authors);
            _connection.DeleteCategries(_categories);
        }
    }
}
