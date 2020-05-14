using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using Bogus;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Ports.Database;

namespace Inshapardaz.Functions.Tests.DataBuilders
{
    public class SeriesDataBuilder
    {
        private readonly IDbConnection _connection;
        private List<AuthorDto> _authors = new List<AuthorDto>();
        private List<BookDto> _books = new List<BookDto>();
        private List<SeriesDto> _series = new List<SeriesDto>();
        private List<FileDto> _files = new List<FileDto>();
        private readonly FakeFileStorage _fileStorage;
        private bool _withImage = true;
        private int _bookCount;
        private int _libraryId;
        private string _namePattern = "";

        public IEnumerable<BookDto> Books => _books;
        public IEnumerable<SeriesDto> Series => _series;

        public SeriesDataBuilder(IProvideConnection connectionProvider, IFileStorage fileStorage)
        {
            _connection = connectionProvider.GetConnection();
            _fileStorage = fileStorage as FakeFileStorage;
        }

        public SeriesDataBuilder WithBooks(int bookCount)
        {
            _bookCount = bookCount;
            return this;
        }

        public SeriesDataBuilder WithNamePattern(string pattern)
        {
            _namePattern = pattern;
            return this;
        }

        internal SeriesDataBuilder WithLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public SeriesDataBuilder WithoutImage()
        {
            _withImage = false;
            return this;
        }

        public SeriesDto Build() => Build(1).Single();

        public IEnumerable<SeriesDto> Build(int count)
        {
            var fixture = new Fixture();

            for (int i = 0; i < count; i++)
            {
                var author = fixture.Build<AuthorDto>()
                                     .With(a => a.LibraryId, _libraryId)
                                     .Without(a => a.ImageId)
                                     .Create();

                _connection.AddAuthor(author);
                _authors.Add(author);

                FileDto seriesImage = null;
                if (_withImage)
                {
                    seriesImage = fixture.Build<FileDto>()
                                         .With(a => a.FilePath, Helpers.Random.BlobUrl)
                                         .With(a => a.IsPublic, true)
                                         .Create();
                    _connection.AddFile(seriesImage);

                    _files.Add(seriesImage);
                    _fileStorage.SetupFileContents(seriesImage.FilePath, Helpers.Random.Bytes);
                    _connection.AddFile(seriesImage);
                }

                var series = fixture.Build<SeriesDto>()
                                .With(a => a.Name, () => fixture.Create(_namePattern))
                                .With(s => s.LibraryId, _libraryId)
                                .With(a => a.ImageId, seriesImage?.Id)
                                .Create();

                _connection.AddSeries(series);
                _series.Add(series);

                var books = fixture.Build<BookDto>()
                                   .With(b => b.AuthorId, author.Id)
                                   .With(b => b.LibraryId, _libraryId)
                                   .Without(b => b.ImageId)
                                   .With(b => b.SeriesId, series.Id)
                                   .CreateMany(_bookCount);
                _books.AddRange(books);
                _connection.AddBooks(books);
            }

            return _series;
        }

        public void CleanUp()
        {
            _connection.DeleteAuthors(_authors);
            _connection.DeleteSeries(_series);
            _connection.DeleteBooks(_books);
            _connection.DeleteFiles(_files);
        }
    }
}
