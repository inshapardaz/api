using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Adapters.Repositories;

namespace Inshapardaz.Api.Tests.Framework.DataBuilders
{
    public class SeriesDataBuilder
    {
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
        private IAuthorTestRepository _authorRepository;
        private IFileTestRepository _fileRepository;
        private ISeriesTestRepository _seriesRepository;
        private IBookTestRepository _bookRepository;

        public SeriesDataBuilder(IFileStorage fileStorage,
                IAuthorTestRepository authorRepository,
                IFileTestRepository fileRepository,
                ISeriesTestRepository seriesRepository,
                IBookTestRepository bookRepository)
        {
            _fileStorage = fileStorage as FakeFileStorage;
            _authorRepository = authorRepository;
            _fileRepository = fileRepository;
            _seriesRepository = seriesRepository;
            _bookRepository = bookRepository;
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
            var seriesList = new List<SeriesDto>();
            var fixture = new Fixture();

            for (int i = 0; i < count; i++)
            {
                var author = fixture.Build<AuthorDto>()
                                     .With(a => a.LibraryId, _libraryId)
                                     .Without(a => a.ImageId)
                                     .Create();

                _authorRepository.AddAuthor(author);
                _authors.Add(author);

                FileDto seriesImage = null;
                if (_withImage)
                {
                    seriesImage = fixture.Build<FileDto>()
                                         .With(a => a.FilePath, Framework.Helpers.RandomData.FilePath)
                                         .With(a => a.IsPublic, true)
                                         .Create();
                    _fileRepository.AddFile(seriesImage);

                    _files.Add(seriesImage);
                    _fileStorage.SetupFileContents(seriesImage.FilePath, Framework.Helpers.RandomData.Bytes);
                }

                var series = fixture.Build<SeriesDto>()
                                .With(a => a.Name, () => fixture.Create(_namePattern))
                                .With(s => s.LibraryId, _libraryId)
                                .With(a => a.ImageId, seriesImage?.Id)
                                .Create();

                _seriesRepository.AddSeries(series);
                seriesList.Add(series);

                var books = fixture.Build<BookDto>()
                                   .With(b => b.LibraryId, _libraryId)
                                   .With(b => b.Language, RandomData.Locale)
                                   .Without(b => b.ImageId)
                                   .With(b => b.SeriesId, series.Id)
                                   .CreateMany(_bookCount);
                _books.AddRange(books);
                _bookRepository.AddBooks(books);
                _bookRepository.AddBooksAuthor(books.Select(b => b.Id), author.Id);
            }

            _series.AddRange(seriesList);
            return seriesList;
        }

        public void CleanUp()
        {
            _seriesRepository.DeleteSeries(_series);
            _fileRepository.DeleteFiles(_files);
            _bookRepository.DeleteBooks(_books);
            _authorRepository.DeleteAuthors(_authors);
        }
    }
}
