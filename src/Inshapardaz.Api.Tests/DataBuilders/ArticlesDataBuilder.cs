using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Fakes;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Database.SqlServer;
using RandomData = Inshapardaz.Api.Tests.Helpers.RandomData;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Tests.DataBuilders
{

    public class ArticlesDataBuilder
    {
        private class AccountItemCountSpec
        {
            public int AccountId { get; set; }
            public int? Count { get; set; }
        }

        private readonly IDbConnection _connection;
        private readonly AuthorsDataBuilder _authorBuilder;
        private readonly CategoriesDataBuilder _categoriesBuilder;
        private readonly FakeFileStorage _fileStorage;

        private List<ArticleDto> _articles;
        private readonly List<FileDto> _files = new();
        private List<AuthorDto> _authors = new();

        private bool _hasImage = true;
        private bool? _isPublic = null;
        private int _categoriesCount, _contentCount;
        private string _contentMimeType;

        public AuthorDto Author { get; set; }
        private List<CategoryDto> _categories = new();
        private int _libraryId;
        private List<AccountItemCountSpec> _favoriteArticles = new();
        private List<AccountItemCountSpec> _readArticles = new();
        private List<ArticleContentDto> _contents = new();
        private string _language = null;
        private int _numberOfAuthors;
        private List<RecentArticleDto> _recentArticles = new();
        private Dictionary<int, int> _writerAssignments = new();
        private Dictionary<int, int> _reviewerAssignments = new();
        private Dictionary<EditingStatus, int> _pageStatuses = new();

        public IEnumerable<AuthorDto> Authors => _authors;
        public IEnumerable<ArticleDto> Articles => _articles;
        public IEnumerable<ArticleContentDto> Contents => _contents;
        public IEnumerable<RecentArticleDto> RecentReads => _recentArticles;

        public ArticlesDataBuilder(IProvideConnection connectionProvider, IFileStorage fileStorage,
                                AuthorsDataBuilder authorBuilder, CategoriesDataBuilder categoriesBuilder)
        {
            _connection = connectionProvider.GetConnection();
            _fileStorage = fileStorage as FakeFileStorage;
            _authorBuilder = authorBuilder;
            _categoriesBuilder = categoriesBuilder;
        }

        internal ArticlesDataBuilder IsPublic(bool isPublic = true)
        {
            _isPublic = isPublic;
            return this;
        }

        public ArticlesDataBuilder WithCategories(int categoriesCount)
        {
            _categoriesCount = categoriesCount;
            return this;
        }

        public ArticlesDataBuilder WithCategory(CategoryDto category)
        {
            _categories.Add(category);
            return this;
        }

        public ArticlesDataBuilder WithCategories(IEnumerable<CategoryDto> categories)
        {
            _categories = categories.ToList();
            return this;
        }

        public ArticlesDataBuilder WithNoImage()
        {
            _hasImage = false;
            return this;
        }

        public ArticlesDataBuilder WithAuthor(AuthorDto author)
        {
            Author = author;
            return this;
        }

        public ArticlesDataBuilder WithAuthors(IEnumerable<AuthorDto> authors)
        {
            _authors = authors.ToList();
            return this;
        }

        public ArticlesDataBuilder WithAuthors(int numberOfAuthors)
        {
            _numberOfAuthors = numberOfAuthors;
            return this;
        }

        internal ArticlesDataBuilder WithLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public ArticlesDataBuilder WithStatus(EditingStatus statuses, int count)
        {
            _pageStatuses.TryAdd(statuses, count);
            return this;
        }

        internal ArticlesDataBuilder WithContentLanguage(string language)
        {
            _language = language;
            return this;
        }

        public ArticlesDataBuilder AddToFavorites(int accountId, int? countOfbookToAddToFavorite = null)
        {
            _favoriteArticles.Add(new AccountItemCountSpec() { AccountId = accountId, Count = countOfbookToAddToFavorite });
            return this;
        }

        public ArticlesDataBuilder AddToRecentReads(int accountId, int? countOfBookToAddToRecent = null)
        {
            _readArticles.Add(new AccountItemCountSpec() { AccountId = accountId, Count = countOfBookToAddToRecent });

            return this;
        }

        public ArticlesDataBuilder WithContent()
        {
            _contentCount = 1;
            return this;
        }

        public ArticlesDataBuilder WithContents(int contentCount, string mimeType = null)
        {
            _contentCount = contentCount;
            _contentMimeType = mimeType;
            return this;
        }

        internal ArticleView BuildView()
        {
            var fixture = new Fixture();

            if (Author == null)
            {
                Author = _authorBuilder.WithLibrary(_libraryId).Build(1).Single();
            }

            return fixture.Build<ArticleView>()
                          .With(a => a.Authors, new List<AuthorView> { new AuthorView { Id = Author.Id } })
                          .With(a => a.Categories, _categories.Any() ? _categories.Select(c => c.ToView()) : new CategoryView[0])
                          .Create();
        }

        public ArticleDto Build()
        {
            return Build(1).Single();
        }

        public IEnumerable<ArticleDto> Build(int numberOfArticles)
        {
            var fixture = new Fixture();

            if (Author == null && !_authors.Any())
            {
                _authors = _authorBuilder.WithLibrary(_libraryId)
                    .Build(_numberOfAuthors > 0 ? _numberOfAuthors : numberOfArticles)
                    .ToList();
            }

            Func<bool> isPublic = () => _isPublic ?? RandomData.Bool;

            _articles = fixture.Build<ArticleDto>()
                          .With(b => b.LibraryId, _libraryId)
                          .With(b => b.ImageId, _hasImage ? RandomData.Number : 0)
                          .With(b => b.IsPublic, isPublic)
                          .With(b => b.LastModified, RandomData.Date)
                          .With(b => b.Status, EditingStatus.Completed)
                          .Without(b => b.WriterAccountId)
                          .Without(b => b.WriterAssignTimeStamp)
                          .Without(b => b.ReviewerAccountId)
                          .Without(b => b.ReviewerAssignTimeStamp)
                          .CreateMany(numberOfArticles)
                          .ToList();

            IEnumerable<CategoryDto> categories;

            if (_categoriesCount > 0 && !_categories.Any())
            {
                categories = _categoriesBuilder
                    .WithLibrary(_libraryId)
                    .Build(_categoriesCount);
            }
            else
            {
                categories = _categories;
            }

            foreach (var article in _articles)
            {
                FileDto articleImage = null;
                if (_hasImage)
                {
                    articleImage = fixture.Build<FileDto>()
                                         .With(a => a.FilePath, RandomData.BlobUrl)
                                         .With(a => a.IsPublic, true)
                                         .Create();
                    _connection.AddFile(articleImage);

                    _files.Add(articleImage);
                    _fileStorage.SetupFileContents(articleImage.FilePath, RandomData.Bytes);
                    _connection.AddFile(articleImage);

                    article.ImageId = articleImage.Id;
                }
                else
                {
                    article.ImageId = null;
                }

                _connection.AddArticle(article);

                if (Author != null)
                {
                    _connection.AddArticleAuthor(article.Id, Author.Id);
                }
                else
                {
                    foreach (var author in _authors)
                    {
                        _connection.AddArticleAuthor(article.Id, author.Id);
                    }
                }

                if (categories != null && categories.Any())
                {
                    _connection.AddArticleToCategories(article.Id, categories);
                }

                if (_contentCount > 0)
                {
                    var contents = Enumerable.Range(1, _contentCount).Select(f => new ArticleContentDto
                    {
                        ArticleId = article.Id,
                        Language = _language ?? RandomData.NextLocale(),
                        Text = RandomData.Words(100),
                    }).ToList();
                    contents.ForEach(f => {
                        f.Id = _connection.AddArticleContents(f);
                    });
                    _contents.AddRange(contents);
                }

                

            }

            if (_favoriteArticles.Any())
            {
                foreach (var f in _favoriteArticles)
                {
                    var articlessToAddToFavorite = f.Count.HasValue ? _articles.PickRandom(f.Count.Value) : _articles;
                    if (f.AccountId != 0)
                        _connection.AddArticlesToFavorites(_libraryId, articlessToAddToFavorite.Select(b => b.Id), f.AccountId);
                }
            }

            if (_readArticles.Any())
            {
                foreach (var r in _readArticles)
                {
                    var articlesToAddToRecent = r.Count.HasValue ? _articles.PickRandom(r.Count.Value) : _articles;
                    foreach (var recentArticle in articlesToAddToRecent)
                    {
                        if (r.AccountId != 0)
                        {
                            RecentArticleDto recent = new RecentArticleDto { LibraryId = _libraryId, ArticleId = recentArticle.Id, AccountId = r.AccountId, DateRead = RandomData.Date };
                            _connection.AddArticleToRecentReads(recent);
                            _recentArticles.Add(recent);
                        }
                    }
                }
            }

            return _articles;
        }

        public void CleanUp()
        {
            _connection.DeleteArticles(_articles);
            _connection.DeleteFiles(_files);
            _authorBuilder.CleanUp();
        }
    }
}
