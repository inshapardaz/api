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
    public class SeriesDataBuilder
    {
        private readonly IDbConnection _connection;
        private List<AuthorDto> _authors = new List<AuthorDto>();
        private List<SeriesDto> _series = new List<SeriesDto>();
        public LibraryDto Library { get; private set; }
        private int _bookCount;

        public SeriesDataBuilder(IProvideConnection connectionProvider)
        {
            _connection = connectionProvider.GetConnection();
        }

        public SeriesDataBuilder WithBooks(int bookCount)
        {
            _bookCount = bookCount;
            return this;
        }

        public SeriesDto Build() => Build(1).Single();

        public IEnumerable<SeriesDto> Build(int count)
        {
            var fixture = new Fixture();
            Library = fixture.Build<LibraryDto>()
                                 .With(l => l.Language, "en")
                                 .Create();

            _connection.AddLibrary(Library);

            var authorGenerator = new Faker<Author>()
                                  .RuleFor(c => c.Id, 0)
                                  .RuleFor(c => c.Name, f => f.Random.AlphaNumeric(10));
            var series = fixture.Build<SeriesDto>()
                                .With(s => s.LibraryId, Library.Id)
                                .Without(s => s.ImageId)
                                .CreateMany(count);

            _connection.AddSerieses(series);
            _series.AddRange(series);

            foreach (var s in series)
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
                                   .With(b => b.SeriesId, s.Id)
                                   .CreateMany(_bookCount);
                _connection.AddBooks(books);
            }

            return series;
        }

        public void CleanUp()
        {
            if (Library != null)
                _connection.DeleteLibrary(Library.Id);

            _connection.DeleteAuthors(_authors);
            _connection.DeleteSeries(_series);
        }
    }
}
