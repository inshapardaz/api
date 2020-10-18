using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Fakes;
using Inshapardaz.Database.SqlServer;
using Inshapardaz.Api.Tests.Helpers;

namespace Inshapardaz.Api.Tests.DataBuilders
{
    public class AuthorsDataBuilder
    {
        private List<AuthorDto> _authors = new List<AuthorDto>();
        private List<BookDto> _books = new List<BookDto>();
        private List<FileDto> _files = new List<FileDto>();
        private readonly IDbConnection _connection;
        private readonly FakeFileStorage _fileStorage;
        private int _libraryId;
        private int _bookCount;
        private bool _withImage = true;
        private string _namePattern = "";

        internal IEnumerable<BookDto> Books => _books;

        public IEnumerable<AuthorDto> Authors => _authors;

        public AuthorsDataBuilder(IProvideConnection connectionProvider, IFileStorage fileStorage)
        {
            _connection = connectionProvider.GetConnection();
            _fileStorage = fileStorage as FakeFileStorage;
        }

        public AuthorsDataBuilder WithNamePattern(string pattern)
        {
            _namePattern = pattern;
            return this;
        }

        public AuthorsDataBuilder WithLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
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

        public AuthorDto Build()
        {
            return Build(1).Single();
        }

        public IEnumerable<AuthorDto> Build(int count)
        {
            var fixture = new Fixture();

            var authors = new List<AuthorDto>();

            for (int i = 0; i < count; i++)
            {
                FileDto authorImage = null;
                if (_withImage)
                {
                    authorImage = fixture.Build<FileDto>()
                                         .With(a => a.FilePath, Helpers.Random.BlobUrl)
                                         .With(a => a.IsPublic, true)
                                         .Create();
                    _connection.AddFile(authorImage);

                    _files.Add(authorImage);
                    _fileStorage.SetupFileContents(authorImage.FilePath, Helpers.Random.Bytes);
                    _connection.AddFile(authorImage);
                }

                var author = fixture.Build<AuthorDto>()
                                     .With(a => a.Name, () => fixture.Create(_namePattern))
                                     //.With(a => a.Name, Random.Name)
                                     .With(a => a.LibraryId, _libraryId)
                                     .With(a => a.ImageId, authorImage?.Id)
                                     .Create();

                _connection.AddAuthor(author);
                _authors.Add(author);

                var books = fixture.Build<BookDto>()
                                   .With(b => b.LibraryId, _libraryId)
                                   .With(b => b.AuthorId, author.Id)
                                   .With(b => b.Language, Random.Locale)
                                   .Without(b => b.ImageId)
                                   .Without(b => b.SeriesId)
                                   .CreateMany(_bookCount);
                _connection.AddBooks(books);

                _books.AddRange(books);

                authors.Add(author);
            }

            return authors;
        }

        public void CleanUp()
        {
            _connection.DeleteAuthors(_authors);
            _connection.DeleteFiles(_files);
        }
    }
}
