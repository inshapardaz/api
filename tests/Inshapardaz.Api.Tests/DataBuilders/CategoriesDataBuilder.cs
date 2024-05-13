using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Adapters.Database.SqlServer;
using Inshapardaz.Domain.Adapters;

namespace Inshapardaz.Api.Tests.DataBuilders
{
    public class CategoriesDataBuilder
    {
        private readonly IDbConnection _connection;
        private int _bookCount, _priodicalCount;
        private List<AuthorDto> _authors = new List<AuthorDto>();
        private List<CategoryDto> _categories = new List<CategoryDto>();
        private int _libraryId;
        private IEnumerable<BookDto> _books;
        private IEnumerable<PeriodicalDto> _periodicals;

        public CategoriesDataBuilder(IProvideConnection connectionProvider)
        {
            _connection = connectionProvider.GetConnection();
        }

        public CategoriesDataBuilder WithBooks(int bookCount)
        {
            _bookCount = bookCount;
            return this;
        }

        public CategoriesDataBuilder WithPeriodicals(int count)
        {
            _priodicalCount = count;
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

                _books = fixture.Build<BookDto>()
                                   .With(b => b.LibraryId, _libraryId)
                                   .With(b => b.Language, RandomData.Locale)
                                   .Without(b => b.ImageId)
                                   .Without(b => b.SeriesId)
                                   .CreateMany(_bookCount);
                _connection.AddBooks(_books);

                _periodicals = fixture.Build<PeriodicalDto>()
                                   .With(b => b.LibraryId, _libraryId)
                                   .With(b => b.Language, RandomData.Locale)
                                   .CreateMany(_priodicalCount);
                _connection.AddPeriodicals(_periodicals);

                _connection.AddBooksAuthor(_books.Select(b => b.Id), author.Id);

                _connection.AddBooksToCategory(_books, cat);

                _connection.AddPeriodicalToCategory(_periodicals, cat);
            }

            return cats;
        }

        public void CleanUp()
        {
            _connection.DeleteAuthors(_authors);
            _connection.DeleteCategries(_categories);
            _connection.DeleteBooks(_books);
            _connection.DeletePeriodicals(_periodicals);
        }
    }

    public static class CategoriesDataExtenstions
    {
        public static CategoryView ToView(this CategoryDto dto) => new CategoryView
        {
            Id = dto.Id,
            Name = dto.Name
        };
    }
}
