using AutoFixture;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;

namespace Inshapardaz.Api.Tests.Framework.DataBuilders
{
    public class TagsDataBuilder(
        ITagTestRepository tagRepository,
        IAuthorTestRepository authorRepository,
        IBookTestRepository bookRepository,
        IPeriodicalTestRepository periodicalRepository)
    {
        private int _bookCount, _periodicalCount;
        private List<AuthorDto> _authors = new List<AuthorDto>();
        private List<TagDto> _tags = new List<TagDto>();
        private int _libraryId;
        private IEnumerable<BookDto> _books;
        private IEnumerable<PeriodicalDto> _periodicals;

        public TagsDataBuilder WithBooks(int bookCount)
        {
            _bookCount = bookCount;
            return this;
        }

        public TagsDataBuilder WithPeriodicals(int count)
        {
            _periodicalCount = count;
            return this;
        }

        public TagDto Build() => Build(1).Single();

        internal TagsDataBuilder WithLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public IEnumerable<TagDto> Build(int count)
        {
            var fixture = new Fixture();

            var tags = fixture.Build<TagDto>()
                              .With(c => c.LibraryId, _libraryId)
                               .CreateMany(count);

            tagRepository.AddTags(tags);
            _tags.AddRange(tags);

            foreach (var tag in tags)
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

                tagRepository.AddBooksToTag(_books, tag);

                tagRepository.AddPeriodicalToTag(_periodicals, tag);
            }

            return tags;
        }

        public void CleanUp()
        {
            periodicalRepository.DeletePeriodicals(_periodicals);
            bookRepository.DeleteBooks(_books);
            authorRepository.DeleteAuthors(_authors);
            tagRepository.DeleteTags(_tags);
        }
    }
}
