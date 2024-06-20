using System;
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
    public class BookShelfDataBuilder
    {
        private List<AuthorDto> _authors = new List<AuthorDto>();
        private List<BookDto> _books = new List<BookDto>();
        private List<BookShelfDto> _bookShelf = new List<BookShelfDto>();
        private Dictionary<int, List<BookDto>> _bookShelfBookList = new ();
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
        public Dictionary<int, List<BookDto>> BookShelvesBookList => _bookShelfBookList;

        public IAuthorTestRepository _authorRepository;
        public IFileTestRepository _fileRepository;
        public IBookTestRepository _bookRepository;
        public IBookShelfTestRepository _bookShelfRepository;

        public BookShelfDataBuilder(IFileStorage fileStorage,
            IAuthorTestRepository authorRepository,
            IFileTestRepository fileRepository,
            IBookShelfTestRepository bookShelfRepository,
            IBookTestRepository bookRepository)
        {
            _fileStorage = fileStorage as FakeFileStorage;
            _authorRepository = authorRepository;
            _fileRepository = fileRepository;
            _bookShelfRepository = bookShelfRepository;
            _bookRepository = bookRepository;
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
            if (_accountId == 0) throw new Exception("Please specify accountid in bookshelf setup");
            var bookShelfList = new List<BookShelfDto>();
            var fixture = new Fixture();

            for (int i = 0; i < count; i++)
            {
                var author = fixture.Build<AuthorDto>()
                                     .With(a => a.LibraryId, _libraryId)
                                     .Without(a => a.ImageId)
                                     .Create();

                _authorRepository.AddAuthor(author);
                _authors.Add(author);

                FileDto bookShelfImage = null;
                if (_withImage)
                {
                    bookShelfImage = fixture.Build<FileDto>()
                                         .With(a => a.FilePath, RandomData.FilePath)
                                         .With(a => a.IsPublic, true)
                                         .Create();
                    _fileRepository.AddFile(bookShelfImage);

                    _files.Add(bookShelfImage);
                    _fileStorage.SetupFileContents(bookShelfImage.FilePath, RandomData.Bytes);
                    _fileRepository.AddFile(bookShelfImage);
                }

                var bookShelf = fixture.Build<BookShelfDto>()
                                .With(s => s.Name, () => fixture.Create(_namePattern))
                                .With(s => s.LibraryId, _libraryId)
                                .With(s => s.ImageId, bookShelfImage?.Id)
                                .With(s => s.IsPublic, _isPublic)
                                .With(s => s.AccountId, _accountId)
                                .Create();

                _bookShelfRepository.AddBookShelf(bookShelf);

                bookShelfList.Add(bookShelf);

                var books = fixture.Build<BookDto>()
                                   .With(b => b.LibraryId, _libraryId)
                                   .With(b => b.Language, RandomData.Locale)
                                   .Without(b => b.ImageId)
                                   .Without(b => b.SeriesId)
                                   .Without(b => b.SeriesIndex)
                                   .Without(b => b.Id)
                                   .CreateMany(_bookCount);
                _bookRepository.AddBooks(books);
                _bookShelfRepository.AddBooksToBookShelf(bookShelf.Id, books.Select(b => b.Id));
                _bookShelfBookList.Add(bookShelf.Id, books.ToList());
                _bookRepository.AddBooksAuthor(books.Select(b => b.Id), author.Id);
                _books.AddRange(books);
            }

            _bookShelf.AddRange(bookShelfList);
            return bookShelfList;
        }

        public void CleanUp()
        {
            _authorRepository.DeleteAuthors(_authors);
            _bookRepository.DeleteBooks(_books);
            _fileRepository.DeleteFiles(_files);
            _bookShelfRepository.DeleteBookShelf(_bookShelf);
        }
    }
}
