using AutoFixture;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;

namespace Inshapardaz.Api.Tests.Framework.DataBuilders
{
    public class CategoriesDataBuilder(
        ICategoryTestRepository categoryRepository,
        IAuthorTestRepository authorRepository,
        IBookTestRepository bookRepository,
        IPeriodicalTestRepository periodicalRepository)
    {
        private int _bookCount, _periodicalCount;
        private List<AuthorDto> _authors = new List<AuthorDto>();
        private List<CategoryDto> _categories = new List<CategoryDto>();
        private int _libraryId;
        private IEnumerable<BookDto> _books;
        private IEnumerable<PeriodicalDto> _periodicals;

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

            categoryRepository.AddCategories(cats);
            _categories.AddRange(cats);

            foreach (var cat in cats)
            {
                var author = fixture.Build<AuthorDto>()
                                     .With(a => a.LibraryId, _libraryId)
                                     .Without(a => a.ImageId)
                                     .Create();

                authorRepository.AddAuthor(author);
                _authors.Add(author);

                _books = fixture.Build<BookDto>()
                                   .With(b => b.LibraryId, _libraryId)
                                   .With(b => b.Language, RandomData.Locale)
                                   .Without(b => b.ImageId)
                                   .Without(b => b.SeriesId)
                                   .CreateMany(_bookCount);
                bookRepository.AddBooks(_books);

                _periodicals = fixture.Build<PeriodicalDto>()
                                   .With(b => b.LibraryId, _libraryId)
                                   .With(b => b.Language, RandomData.Locale)
                                   .CreateMany(_periodicalCount);
                periodicalRepository.AddPeriodicals(_periodicals);

                bookRepository.AddBooksAuthor(_books.Select(b => b.Id), author.Id);

                categoryRepository.AddBooksToCategory(_books, cat);

                categoryRepository.AddPeriodicalToCategory(_periodicals, cat);
            }

            return cats;
        }

        public void CleanUp()
        {
            periodicalRepository.DeletePeriodicals(_periodicals);
            bookRepository.DeleteBooks(_books);
            authorRepository.DeleteAuthors(_authors);
            categoryRepository.DeleteCategories(_categories);
        }
    }
}
