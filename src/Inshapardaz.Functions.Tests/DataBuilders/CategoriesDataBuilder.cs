using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Ports.Database;

namespace Inshapardaz.Functions.Tests.DataBuilders
{
    public class CategoriesDataBuilder
    {
        private readonly IDbConnection _connection;
        private int _bookCount;
        private List<AuthorDto> _authors = new List<AuthorDto>();
        private List<CategoryDto> _categories = new List<CategoryDto>();
        private int _libraryId;

        public CategoriesDataBuilder(IProvideConnection connectionProvider)
        {
            _connection = connectionProvider.GetConnection();
        }

        public CategoriesDataBuilder WithBooks(int bookCount)
        {
            _bookCount = bookCount;
            return this;
        }

        public CategoryDto Build() => Build(1).Single();

        internal CategoriesDataBuilder WithLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public IEnumerable<CategoryDto> Build(int count)
        {
            var fixture = new Fixture();

            var cats = fixture.Build<CategoryDto>()
                              .With(c => c.LibraryId, _libraryId)
                               .CreateMany(count);

            _connection.AddCategories(cats);
            _categories.AddRange(cats);

            foreach (var cat in cats)
            {
                var author = fixture.Build<AuthorDto>()
                                     .With(a => a.LibraryId, _libraryId)
                                     .Without(a => a.ImageId)
                                     .Create();

                _connection.AddAuthor(author);
                _authors.Add(author);

                var books = fixture.Build<BookDto>()
                                   .With(b => b.AuthorId, author.Id)
                                   .With(b => b.LibraryId, _libraryId)
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
            _connection.DeleteAuthors(_authors);
            _connection.DeleteCategries(_categories);
        }
    }
}
