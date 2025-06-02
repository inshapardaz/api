using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AutoFixture;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using RandomData = Inshapardaz.Api.Tests.Framework.Helpers.RandomData;
using Inshapardaz.Domain.Models;
using Bogus;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Adapters.Repositories;

namespace Inshapardaz.Api.Tests.Framework.DataBuilders
{

    public class ArticlesDataBuilder
    {
        private class AccountItemCountSpec
        {
            public int AccountId { get; set; }
            public int? Count { get; set; }
        }

        private readonly ICategoryTestRepository _categoryRepository;
        private readonly IArticleTestRepository _articleRepository;
        private readonly ITagTestRepository _tagRepository;
        private readonly IFileTestRepository _fileRepository;
        private readonly AuthorsDataBuilder _authorBuilder;
        private readonly CategoriesDataBuilder _categoriesBuilder;
        private readonly TagsDataBuilder _tagsBuilder;
        private readonly FakeFileStorage _fileStorage;

        private readonly List<ArticleDto> _articles = new();
        private readonly List<FileDto> _files = new();
        private List<AuthorDto> _authors = new();

        private bool _hasImage = true;
        private bool? _isPublic = null;
        private int _categoriesCount, _tagsCount, _contentCount;

        private AuthorDto Author { get; set; }
        private List<CategoryDto> _categories = new();
        private List<TagDto> _tags = new List<TagDto>();
        private int _libraryId;
        private List<AccountItemCountSpec> _favoriteArticlesSpec = new();
        private List<AccountItemCountSpec> _readArticlesSpec = new();
        private List<ArticleContentDto> _contents = new();
        private string _language = null;
        private int _numberOfAuthors;
        private List<RecentArticleDto> _recentArticles = new();
        private List<FavoriteArticleDto> _favoriteArticles = new();
        private int? _assignedWriterId, _assignedReviewerId;
        private EditingStatus? _status;
        private ArticleType? _articleType;
        private Dictionary<long, List<int>> _articleTags = new();
        private Dictionary<long, List<int>> _articleCategories = new();

        public IEnumerable<AuthorDto> Authors => _authors;
        public IEnumerable<ArticleDto> Articles => _articles;
        public IEnumerable<FavoriteArticleDto> FavoriteArticles => _favoriteArticles;
        public IEnumerable<ArticleContentDto> Contents => _contents;
        public IEnumerable<FileDto> Files => _files;
        public IEnumerable<RecentArticleDto> RecentReads => _recentArticles;
        public Dictionary<long, List<int>> ArticleCategories => _articleCategories;
        public Dictionary<long, List<int>> ArticleTags => _articleTags;

        public ArticlesDataBuilder(
            ICategoryTestRepository categoryRepository,
            IArticleTestRepository articleRepository,
            TagsDataBuilder tagsBuilder,
            IFileStorage fileStorage, 
            IFileTestRepository fileRepository,
            ITagTestRepository tagRepository, 
            AuthorsDataBuilder authorBuilder, 
            CategoriesDataBuilder categoriesBuilder)
        {
            _fileStorage = fileStorage as FakeFileStorage;
            _categoryRepository = categoryRepository;
            _tagsBuilder = tagsBuilder;
            _articleRepository = articleRepository;
            _fileRepository = fileRepository;
            _tagRepository = tagRepository;
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

        public ArticlesDataBuilder WithTags(int tagsCount)
        {
            _tagsCount = tagsCount;
            return this;
        }
        
        public ArticlesDataBuilder WithTag(TagDto tag)
        {
            _tags.Add(tag);
            return this;
        }
        
        
        public ArticlesDataBuilder WithTags(IEnumerable<TagDto> tags)
        {
            _tags = tags.ToList();
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

        public ArticlesDataBuilder WithStatus(EditingStatus statuses)
        {
            _status = statuses;
            return this;
        }

        internal ArticlesDataBuilder WithContentLanguage(string language)
        {
            _language = language;
            return this;
        }

        public ArticlesDataBuilder AddToFavorites(int accountId, int? countOfArticlesToAddToFavorite = null)
        {
            _favoriteArticlesSpec.Add(new AccountItemCountSpec() { AccountId = accountId, Count = countOfArticlesToAddToFavorite });
            return this;
        }

        public ArticlesDataBuilder AddToRecentReads(int accountId, int? countOfArticlesToAddToRecent = null)
        {
            _readArticlesSpec.Add(new AccountItemCountSpec() { AccountId = accountId, Count = countOfArticlesToAddToRecent });

            return this;
        }

        public ArticlesDataBuilder WithContent()
        {
            _contentCount = 1;
            return this;
        }

        public ArticlesDataBuilder WithContents(int contentCount)
        {
            _contentCount = contentCount;
            return this;
        }


        internal ArticlesDataBuilder WithType(ArticleType type)
        {
            _articleType = type;
            return this;
        }

        internal ArticlesDataBuilder WithWtiterAssignment(int accountId)
        {
            _assignedWriterId = accountId;
            return this;
        }

        internal ArticlesDataBuilder WithReviewerAssignment(int accountId)
        {
            _assignedReviewerId = accountId;
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
                          .With(b => b.Tags, _tags.Any() ? _tags.Select(c => c.ToView()) : new TagView[0])
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

            var articles = fixture.Build<ArticleDto>()
                          .With(b => b.LibraryId, _libraryId)
                          .With(b => b.ImageId, () => _hasImage ? RandomData.Number : 0)
                          .With(b => b.IsPublic, isPublic)
                          .With(b => b.LastModified, () => RandomData.Date)
                          .With(b => b.Status, _status ?? EditingStatus.Completed)
                          .With(b => b.Type, _articleType ?? new Faker().PickRandom(ArticleType.Writing, ArticleType.Poetry))
                          .With(b => b.WriterAccountId, _assignedWriterId)
                          .With(b => b.WriterAssignTimeStamp, _assignedWriterId.HasValue ? DateTime.Now : null)
                          .With(b => b.ReviewerAccountId, _assignedReviewerId)
                          .With(b => b.ReviewerAssignTimeStamp, _assignedReviewerId.HasValue ? DateTime.Now : null)
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
            
            IEnumerable<TagDto> tags;

            if (_tagsCount > 0 && !_tags.Any())
            {
                tags = _tagsBuilder.WithLibrary(_libraryId).Build(_tagsCount);
            }
            else
            {
                tags = _tags;
            }
            

            foreach (var article in articles)
            {
                FileDto articleImage = null;
                if (_hasImage)
                {
                    articleImage = fixture.Build<FileDto>()
                                         .With(a => a.FilePath, RandomData.FilePath)
                                         .With(a => a.IsPublic, true)
                                         .Create();
                    _fileRepository.AddFile(articleImage);

                    _files.Add(articleImage);
                    _fileStorage.SetupFileContents(articleImage.FilePath, RandomData.Bytes);

                    article.ImageId = articleImage.Id;
                }
                else
                {
                    article.ImageId = null;
                }

                _articleRepository.AddArticle(article);

                if (Author != null)
                {
                    _articleRepository.AddArticleAuthor(article.Id, Author.Id);
                }
                else
                {
                    foreach (var author in _authors)
                    {
                        _articleRepository.AddArticleAuthor(article.Id, author.Id);
                    }
                }

                if (categories != null && categories.Any())
                {
                    _categoryRepository.AddArticleToCategories(article.Id, categories);
                    _articleCategories.Add(article.Id, categories.Select(x => x.Id).ToList());
                }

                if (tags != null && tags.Any())
                {
                    _tagRepository.AddArticleToTags(article.Id, tags);
                    _articleTags.Add(article.Id, tags.Select(x => x.Id).ToList());
                }

                if (_contentCount > 0)
                {
                    var contents = Enumerable.Range(1, _contentCount).Select(f => new ArticleContentDto
                    {
                        ArticleId = article.Id,
                        Language = _language ?? RandomData.NextLocale(),
                        Layout = RandomData.String
                    }).ToList();
                    contents.ForEach(ac =>
                    {
                        var articleContentFile = fixture.Build<FileDto>()
                                    .With(a => a.FilePath, RandomData.FilePath)
                                    .With(a => a.IsPublic, false)
                                    .With(a => a.MimeType, MimeTypes.Markdown)
                                    .Create();
                        _fileRepository.AddFile(articleContentFile);
                        _files.Add(articleContentFile);

                        var articleContentData = RandomData.Text;
                        _fileStorage.SetupFileContents(articleContentFile.FilePath, articleContentData);
                        ac.FileId = articleContentFile.Id;

                        ac.Id = _articleRepository.AddArticleContents(ac);
                    });
                    _contents.AddRange(contents);
                }



            }

            if (_favoriteArticlesSpec.Any())
            {
                foreach (var f in _favoriteArticlesSpec)
                {
                    var articlessToAddToFavorite = f.Count.HasValue ? articles.PickRandom(f.Count.Value) : articles;
                    foreach (var fav in articlessToAddToFavorite)
                    {
                        if (f.AccountId != 0)
                        {
                            var favorite = new FavoriteArticleDto { AccountId = f.AccountId, ArticleId = fav.Id, LibraryId = _libraryId, DateAdded = RandomData.Date };
                            _articleRepository.AddArticleToFavorites(_libraryId, favorite.ArticleId, favorite.AccountId, favorite.DateAdded);

                            _favoriteArticles.Add(favorite);
                        }
                    }
                }
            }

            if (_readArticlesSpec.Any())
            {
                foreach (var r in _readArticlesSpec)
                {
                    var articlesToAddToRecent = r.Count.HasValue ? articles.PickRandom(r.Count.Value) : articles;
                    foreach (var recentArticle in articlesToAddToRecent)
                    {
                        if (r.AccountId != 0)
                        {
                            RecentArticleDto recent = new RecentArticleDto { LibraryId = _libraryId, ArticleId = recentArticle.Id, AccountId = r.AccountId, DateRead = RandomData.Date };
                            _articleRepository.AddArticleToRecentReads(recent);
                            _recentArticles.Add(recent);
                        }
                    }
                }
            }

            _articles.AddRange(articles);
            return _articles;
        }

        public void CleanUp()
        {
            _articleRepository.DeleteArticles(_articles);
            _fileRepository.DeleteFiles(_files);
            _authorBuilder.CleanUp();
        }
    }
}
