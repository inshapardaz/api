using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Ports.Database;

namespace Inshapardaz.Functions.Tests.DataBuilders
{
    public class AuthorsDataBuilder
    {
        private List<AuthorDto> _authors = new List<AuthorDto>();
        private List<FileDto> _files = new List<FileDto>();
        private readonly IDbConnection _connection;
        private int _libraryId;
        private int _bookCount;
        private bool _withImage = true;

        public AuthorsDataBuilder(IProvideConnection connectionProvider)
        {
            _connection = connectionProvider.GetConnection();
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
                    authorImage = fixture.Create<FileDto>();
                    _connection.AddFile(authorImage);

                    _files.Add(authorImage);
                }

                var author = fixture.Build<AuthorDto>()
                                     .With(a => a.LibraryId, _libraryId)
                                     .With(a => a.ImageId, authorImage.Id)
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
