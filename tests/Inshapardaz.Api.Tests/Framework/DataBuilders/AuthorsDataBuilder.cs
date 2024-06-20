using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Adapters.Repositories;

namespace Inshapardaz.Api.Tests.Framework.DataBuilders
{
    public class AuthorsDataBuilder
    {
        private List<AuthorDto> _authors = new();
        private List<BookDto> _books = new();
        private List<FileDto> _files = new();
        private List<ArticleDto> _articles = new();
        private readonly FakeFileStorage _fileStorage;
        private readonly IFileTestRepository _fileRepository;
        private readonly IBookTestRepository _bookTestRepository;
        private readonly IAuthorTestRepository _authorTestRepository;
        private readonly IArticleTestRepository _articleTestRepository;
        private int _libraryId;
        private int _bookCount;
        private bool _withImage = true;
        private string _namePattern = "";
        private int _articleCount;

        internal IEnumerable<BookDto> Books => _books;

        public IEnumerable<AuthorDto> Authors => _authors;

        public AuthorsDataBuilder(IFileStorage fileStorage,
            IFileTestRepository fileRepository,
            IBookTestRepository bookTestRepository,
            IAuthorTestRepository authorTestRepository,
            IArticleTestRepository articleTestRepository)
        {
            _fileStorage = fileStorage as FakeFileStorage;
            _fileRepository = fileRepository;
            _bookTestRepository = bookTestRepository;
            _authorTestRepository = authorTestRepository;
            _articleTestRepository = articleTestRepository;
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


        internal AuthorsDataBuilder WithArticles(int count)
        {
            _articleCount = count;
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
                                         .With(a => a.FilePath, RandomData.FilePath)
                                         .With(a => a.IsPublic, true)
                                         .Create();
                    _fileRepository.AddFile(authorImage);
                    _files.Add(authorImage);
                    _fileStorage.SetupFileContents(authorImage.FilePath, Framework.Helpers.RandomData.Bytes);
                }

                var author = fixture.Build<AuthorDto>()
                                     .With(a => a.Name, () => fixture.Create(_namePattern))
                                     .With(a => a.LibraryId, _libraryId)
                                     .With(a => a.ImageId, authorImage?.Id)
                                     .Create();

                _authorTestRepository.AddAuthor(author);
                _authors.Add(author);

                var books = fixture.Build<BookDto>()
                                   .With(b => b.LibraryId, _libraryId)
                                   .With(b => b.Language, RandomData.Locale)
                                   .With(b => b.Status, Domain.Models.StatusType.Published)
                                   .Without(b => b.ImageId)
                                   .Without(b => b.SeriesId)
                                   .CreateMany(_bookCount);
                _bookTestRepository.AddBooks(books);

                _bookTestRepository.AddBooksAuthor(books.Select(b => b.Id), author.Id);

                _books.AddRange(books);

                var articles = fixture.Build<ArticleDto>()
                                .With(b => b.LibraryId, _libraryId)
                                .Without(b => b.ImageId)
                                .With(b => b.IsPublic, true)
                                .With(b => b.LastModified, RandomData.Date)
                                .With(b => b.Status, EditingStatus.Completed)
                                .Without(b => b.WriterAccountId)
                                .Without(b => b.WriterAssignTimeStamp)
                                .Without(b => b.ReviewerAccountId)
                                .Without(b => b.ReviewerAssignTimeStamp)
                                .CreateMany(_articleCount);

                _articleTestRepository.AddArticles(articles);

                _articles.AddRange(articles);

                foreach (var article in articles)
                {
                    _articleTestRepository.AddArticleAuthor(article.Id, author.Id);
                }

                authors.Add(author);
            }

            return authors;
        }

        public void CleanUp()
        {
            _authorTestRepository.DeleteAuthors(_authors);
            _fileRepository.DeleteFiles(_files);
        }

    }
}
