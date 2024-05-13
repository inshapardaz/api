using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Fakes;
using Inshapardaz.Adapters.Database.SqlServer;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Adapters;

namespace Inshapardaz.Api.Tests.DataBuilders
{
    public class BookShelfDataBuilder
    {
        private readonly IDbConnection _connection;
        private List<AuthorDto> _authors = new List<AuthorDto>();
        private List<BookDto> _books = new List<BookDto>();
        private List<BookShelfDto> _bookShelf = new List<BookShelfDto>();
        private List<FileDto> _files = new List<FileDto>();
        private readonly FakeFileStorage _fileStorage;
        private bool _withImage = true;
        private int _accountId;
        private int _bookCount;
        private int _libraryId;
        private string _namePattern = "";
        private bool _isPublic = false;

        public IEnumerable<BookDto> Books => _books;

        public IEnumerable<BookShelfDto> BookShelves => _bookShelf;

        public BookShelfDataBuilder(IProvideConnection connectionProvider, IFileStorage fileStorage)
        {
            _connection = connectionProvider.GetConnection();
            _fileStorage = fileStorage as FakeFileStorage;
        }

        public BookShelfDataBuilder WithBooks(int bookCount)
        {
            _bookCount = bookCount;
            return this;
        }

        public BookShelfDataBuilder WithNamePattern(string pattern)
        {
            _namePattern = pattern;
            return this;
        }

        internal BookShelfDataBuilder WithLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public BookShelfDataBuilder WithoutImage()
        {
            _withImage = false;
            return this;
        }

        public BookShelfDataBuilder ForAccount(int accountId)
        {
            _accountId = accountId;
            return this;
        }

        public BookShelfDataBuilder AsPublic()
        {
            _isPublic = true;
            return this;
        }

        public BookShelfDto Build() => Build(1).Single();

        public IEnumerable<BookShelfDto> Build(int count)
        {
            var bookShelfList = new List<BookShelfDto>();
            var fixture = new Fixture();

            for (int i = 0; i < count; i++)
            {
                var author = fixture.Build<AuthorDto>()
                                     .With(a => a.LibraryId, _libraryId)
                                     .Without(a => a.ImageId)
                                     .Create();

                _connection.AddAuthor(author);
                _authors.Add(author);

                FileDto bookShelfImage = null;
                if (_withImage)
                {
                    bookShelfImage = fixture.Build<FileDto>()
                                         .With(a => a.FilePath, RandomData.BlobUrl)
                                         .With(a => a.IsPublic, true)
                                         .Create();
                    _connection.AddFile(bookShelfImage);

                    _files.Add(bookShelfImage);
                    _fileStorage.SetupFileContents(bookShelfImage.FilePath, RandomData.Bytes);
                    _connection.AddFile(bookShelfImage);
                }

                var bookShelf = fixture.Build<BookShelfDto>()
                                .With(s => s.Name, () => fixture.Create(_namePattern))
                                .With(s => s.LibraryId, _libraryId)
                                .With(s => s.ImageId, bookShelfImage?.Id)
                                .With(s => s.IsPublic, _isPublic)
                                .With(s => s.AccountId, _accountId)
                                .Create();

                _connection.AddBookShelf(bookShelf);

                bookShelfList.Add(bookShelf);

                var books = fixture.Build<BookDto>()
                                   .With(b => b.LibraryId, _libraryId)
                                   .With(b => b.Language, RandomData.Locale)
                                   .Without(b => b.ImageId)
                                   .Without(b => b.SeriesId)
                                   .Without(b => b.SeriesIndex)
                                   .Without(b => b.Id)
                                   .CreateMany(_bookCount);
                _connection.AddBooks(books);
                _connection.AddBookstToBookShelf(bookShelf.Id, books.Select(b => b.Id));
                _connection.AddBooksAuthor(books.Select(b => b.Id), author.Id);
                _books.AddRange(books);
            }

            _bookShelf.AddRange(bookShelfList);
            return bookShelfList;
        }

        public void CleanUp()
        {
            _connection.DeleteAuthors(_authors);
            _connection.DeleteBooks(_books);
            _connection.DeleteFiles(_files);
            _connection.DeleteBookShelf(_bookShelf);
        }
    }
}
