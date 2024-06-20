using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;

namespace Inshapardaz.Api.Tests.Framework.DataBuilders
{
    public class CategoriesDataBuilder
    {
        private int _bookCount, _periodicalCount;
        private List<AuthorDto> _authors = new List<AuthorDto>();
        private List<CategoryDto> _categories = new List<CategoryDto>();
        private int _libraryId;
        private IEnumerable<BookDto> _books;
        private IEnumerable<PeriodicalDto> _periodicals;

        private ICategoryTestRepository _categoryRepository;
        private IAuthorTestRepository _authorRepository;
        private IBookTestRepository _bookRepository;
        private IPeriodicalTestRepository _periodicalRepository;
        public CategoriesDataBuilder(ICategoryTestRepository categoryRepository,
            IAuthorTestRepository authorRepository,
            IBookTestRepository bookRepository,
            IPeriodicalTestRepository periodicalRepository)
        {
            _categoryRepository = categoryRepository;
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
            _periodicalRepository = periodicalRepository;
        }

        public CategoriesDataBuilder WithBooks(int bookCount)
        {
            _bookCount = bookCount;
            return this;
        }

        public CategoriesDataBuilder WithPeriodicals(int count)
        {
            _periodicalCount = count;
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

            _categoryRepository.AddCategories(cats);
            _categories.AddRange(cats);

            foreach (var cat in cats)
            {
                var author = fixture.Build<AuthorDto>()
                                     .With(a => a.LibraryId, _libraryId)
                                     .Without(a => a.ImageId)
                                     .Create();

                _authorRepository.AddAuthor(author);
                _authors.Add(author);

                _books = fixture.Build<BookDto>()
                                   .With(b => b.LibraryId, _libraryId)
                                   .With(b => b.Language, RandomData.Locale)
                                   .Without(b => b.ImageId)
                                   .Without(b => b.SeriesId)
                                   .CreateMany(_bookCount);
                _bookRepository.AddBooks(_books);

                _periodicals = fixture.Build<PeriodicalDto>()
                                   .With(b => b.LibraryId, _libraryId)
                                   .With(b => b.Language, RandomData.Locale)
                                   .CreateMany(_periodicalCount);
                _periodicalRepository.AddPeriodicals(_periodicals);

                _bookRepository.AddBooksAuthor(_books.Select(b => b.Id), author.Id);

                _categoryRepository.AddBooksToCategory(_books, cat);

                _categoryRepository.AddPeriodicalToCategory(_periodicals, cat);
            }

            return cats;
        }

        public void CleanUp()
        {
            _periodicalRepository.DeletePeriodicals(_periodicals);
            _bookRepository.DeleteBooks(_books);
            _authorRepository.DeleteAuthors(_authors);
            _categoryRepository.DeleteCategories(_categories);
        }
    }
}
