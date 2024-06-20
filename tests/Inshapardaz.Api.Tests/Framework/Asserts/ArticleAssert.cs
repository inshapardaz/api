using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class ArticleAssert
    {
        private HttpResponseMessage _response;
        private int _libraryId;
        private readonly IArticleTestRepository _articleRepository;
        private readonly IFileTestRepository _fileRepository;
        private readonly IAuthorTestRepository _authorRepository;
        private readonly ICategoryTestRepository _categoryRepository;
        private readonly FakeFileStorage _fileStorage;
        private ArticleView _article;

        public ArticleAssert(IArticleTestRepository articleRepository, 
            IFileTestRepository fileRepository,
            IAuthorTestRepository authorRepository,
            ICategoryTestRepository categoryRepository,
            FakeFileStorage fileStorage)
        {
            _articleRepository = articleRepository;
            _fileRepository = fileRepository;
            _authorRepository = authorRepository;
            _categoryRepository = categoryRepository;
            _fileStorage = fileStorage;
        }

        public ArticleAssert ForArticleView(ArticleView view)
        {
            _article = view;
            return this;
        }

        public ArticleAssert ForLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public ArticleAssert ForResponse(HttpResponseMessage response)
        {
            _response = response;
            _article = response.GetContent<ArticleView>().Result;
            return this;
        }

        public ArticleAssert ShouldHaveCorrectLocationHeader()
        {
            var location = _response.Headers.Location.AbsoluteUri;
            location.Should().NotBeNull();
            location.Should().EndWith($"libraries/{_libraryId}/articles/{_article.Id}");
            return this;
        }

        public ArticleAssert ShouldBeAssignedToUserForWriting(AccountDto account)
        {
            _article.WriterAccountId.Should().Be(account.Id);
            _article.WriterAccountName.Should().Be(account.Name);
            _article.WriterAssignTimeStamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            return this;
        }

        public ArticleAssert ShouldNotBeAssignedForWriting()
        {
            _article.WriterAccountId.Should().BeNull();
            _article.WriterAccountName.Should().BeNull();
            _article.WriterAssignTimeStamp.Should().BeNull();
            return this;
        }

        public ArticleAssert ShouldBeSavedAssignmentForWriting(AccountDto account)
        {
            var dbArticle = _articleRepository.GetArticleById(_article.Id);
            dbArticle.WriterAccountId.Should().Be(account.Id);
            dbArticle.WriterAssignTimeStamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            return this;
        }

        public ArticleAssert ShouldBeSavedNoAssignmentForWriting()
        {
            var dbArticle = _articleRepository.GetArticleById(_article.Id);
            dbArticle.WriterAccountId.Should().BeNull();
            dbArticle.WriterAssignTimeStamp.Should().BeNull();
            return this;
        }

        public ArticleAssert ShouldBeAssignedToUserForReviewing(AccountDto account)
        {
            _article.ReviewerAccountId.Should().Be(account.Id);
            _article.ReviewerAccountName.Should().Be(account.Name);
            _article.ReviewerAssignTimeStamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            return this;
        }

        public ArticleAssert ShouldNotBeAssignedForReviewing()
        {
            _article.ReviewerAccountId.Should().BeNull();
            _article.ReviewerAccountName.Should().BeNull();
            _article.ReviewerAssignTimeStamp.Should().BeNull();
            return this;
        }

        public ArticleAssert ShouldBeSavedAssignmentForReviewing(AccountDto account)
        {
            var dbArticle = _articleRepository.GetArticleById(_article.Id);
            dbArticle.ReviewerAccountId.Should().Be(account.Id);
            dbArticle.ReviewerAssignTimeStamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            return this;
        }

        public ArticleAssert ShouldBeSavedNoAssignmentForReviewing()
        {
            var dbArticle = _articleRepository.GetArticleById(_article.Id);
            dbArticle.ReviewerAccountId.Should().BeNull();
            dbArticle.ReviewerAssignTimeStamp.Should().BeNull();
            return this;
        }

        public ArticleAssert ShouldHaveSavedArticle()
        {
            var dbArticle = _articleRepository.GetArticleById(_article.Id);
            dbArticle.Should().NotBeNull();
            _article.Title.Should().Be(dbArticle.Title);
            return this;
        }

        public ArticleAssert ShouldHaveDeletedArticle(long articleId)
        {
            var article = _articleRepository.GetArticleById(articleId);
            article.Should().BeNull();
            return this;
        }

        public ArticleAssert ThatContentsAreDeletedForArticle(long articleId)
        {
            var contents = _articleRepository.GetContentByArticle(articleId);
            contents.Should().BeNullOrEmpty();
            return this;
        }

        public ArticleAssert ShouldBeAddedToFavorite(long articleId, int accountId)
        {
            _articleRepository.DoesArticleExistsInFavorites(articleId, accountId).Should().BeTrue();
            return this;
        }

        public ArticleAssert ShouldNotBeInFavorites(long articleId, int accountId)
        {
            _articleRepository.DoesArticleExistsInFavorites(articleId, accountId).Should().BeFalse();
            return this;
        }

        public ArticleAssert ShouldHaveDeletedArticleFromRecentReads(long articleId)
        {
            _articleRepository.DoesArticleExistsInRecent(articleId).Should().BeFalse();
            return this;
        }

        public ArticleAssert ShouldHaveDeletedArticleImage(long articleId, long imageId, string filename)
        {
            var file = _fileRepository.GetFileById(imageId);
            file.Should().BeNull();
            _fileStorage.DoesFileExists(filename);
            return this;
        }

        public ArticleAssert ShouldHaveDeletedArticleContents(long articleId)
        {
            var savedContents = _articleRepository.GetContentByArticle(articleId);
            savedContents.Should().BeNullOrEmpty();
            return this;
        }

        public ArticleAssert ShouldNotHaveUpdatedArticleImage(long articleId, byte[] oldImage)
        {
            var imageUrl = _articleRepository.GetArticleImageUrl(articleId);
            imageUrl.Should().NotBeNull();
            var image = _fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().Equal(oldImage);
            return this;
        }

        public ArticleAssert ShouldHaveAddedArticleImage(long articleId)
        {
            var imageUrl = _articleRepository.GetArticleImageUrl(articleId);
            imageUrl.Should().NotBeNull();
            var image = _fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNullOrEmpty();
            return this;
        }

        public ArticleAssert ShouldHaveUpdatedArticleImage(long articleId, byte[] newImage)
        {
            var imageUrl = _articleRepository.GetArticleImageUrl(articleId);
            imageUrl.Should().NotBeNull();
            var image = _fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNull().And.Equal(newImage);
            return this;
        }

        public ArticleAssert ShouldHaveSelfLink()
        {
            _article.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/articles/{_article.Id}");

            return this;
        }

        public ArticleAssert WithReadOnlyLinks()
        {
            ShouldHaveSelfLink()
            .ShouldNotHaveAddArticleContentLink()
            .ShouldNotHaveUpdateLink()
            .ShouldNotHaveDeleteLink()
            .ShouldNotHaveAssignmentLink();
            return this;
        }

        public ArticleAssert WithWriteableLinks()
        {
            ShouldHaveAddContentLink()
            .ShouldHaveUpdateLink()
            .ShouldHaveDeleteLink()
            .ShouldHaveAssignmentLink();
            return this;
        }

        public void ShouldHaveNoCorrectContents()
        {
            _article.Link("content").Should().BeNull();
        }

        public ArticleAssert ShouldHaveAssignmentLink()
        {
            _article.Link("assign")
                  .ShouldBePost()
                  .EndingWith($"libraries/{_libraryId}/articles/{_article.Id}/assign");
            return this;
        }

        public ArticleAssert ShouldNotHaveAssignmentLink()
        {
            _article.Link("assign")
                  .Should().BeNull();
            return this;
        }

        public ArticleAssert ShouldHaveUpdateLink()
        {
            _article.UpdateLink()
                 .ShouldBePut()
                 .EndingWith($"libraries/{_libraryId}/articles/{_article.Id}");

            return this;
        }

        public ArticleAssert ShouldNotHaveUpdateLink()
        {
            _article.UpdateLink().Should().BeNull();
            return this;
        }

        public ArticleAssert ShouldHaveDeleteLink()
        {
            _article.DeleteLink()
                 .ShouldBeDelete()
                 .EndingWith($"libraries/{_libraryId}/articles/{_article.Id}");

            return this;
        }

        public ArticleAssert ShouldNotHaveDeleteLink()
        {
            _article.DeleteLink().Should().BeNull();
            return this;
        }

        public ArticleAssert ShouldNotHaveAddArticleContentLink()
        {
            _article.Link("add-content").Should().BeNull();
            return this;
        }

        public ArticleAssert ShouldHaveUpdateContentLink(IssueContentDto content)
        {
            var actual = _article.Contents.Single(x => x.Id == content.Id);
            actual.UpdateLink()
                  .ShouldBePut()
                  .EndingWith($"libraries/{_libraryId}/articles/{_article.Id}")
                  .ShouldHaveAcceptLanguage(content.Language);

            return this;
        }

        public ArticleAssert ShouldHaveDeleteContentLink(IssueContentDto content)
        {
            var actual = _article.Contents.Single(x => x.Id == content.Id);
            actual.DeleteLink()
                  .ShouldBeDelete()
                  .EndingWith($"libraries/{_libraryId}/articles/{_article.Id}")
                  .ShouldHaveAcceptLanguage(actual.Language);

            return this;
        }

        public ArticleAssert ShouldNotHaveContentsLink()
        {
            _article.Link("content").Should().BeNull();
            return this;
        }

        public ArticleAssert ShouldHavePublicImageLink()
        {
            _article.Link("image")
                .ShouldBeGet();
            //.Href.Should().StartWith(Settings.CDNAddress);

            return this;
        }

        public ArticleAssert ShouldNotHaveImageUpdateLink()
        {
            _article.Link("image-upload").Should().BeNull();
            return this;
        }

        public ArticleAssert ShouldHaveImageUpdateLink()
        {
            _article.Link("image-upload")
                .ShouldBePut()
                .EndingWith($"libraries/{_libraryId}/articles/{_article.Id}/image");
            return this;
        }

        public ArticleAssert ShouldHaveAddFavoriteLink()
        {
            _article.Link(RelTypes.CreateFavorite)
                  .ShouldBePost()
                  .EndingWith($"libraries/{_libraryId}/favorites/articles/{_article.Id}");

            return this;
        }

        public ArticleAssert ShouldNotHaveAddFavoriteLink()
        {
            _article.Link(RelTypes.CreateFavorite).Should().BeNull();
            return this;
        }

        public ArticleAssert ShouldHaveRemoveFavoriteLink()
        {
            _article.Link(RelTypes.RemoveFavorite)
                  .ShouldBeDelete()
                  .EndingWith($"libraries/{_libraryId}/favorites/articles/{_article.Id}");

            return this;
        }

        public ArticleAssert ShouldNotHaveRemoveFavoriteLink()
        {
            _article.Link(RelTypes.RemoveFavorite).Should().BeNull();
            return this;
        }

        public ArticleAssert ShouldHaveCorrectImageLocationHeader(long articleId)
        {
            _response.Headers.Location.AbsoluteUri.Should().NotBeEmpty();
            return this;
        }

        public ArticleAssert ShouldHavePublicImage(long articleId)
        {
            var image = _articleRepository.GetArticleImage(articleId);
            image.Should().NotBeNull();
            image.IsPublic.Should().BeTrue();
            return this;
        }

        public ArticleAssert ShouldHaveContents(List<ArticleContentDto> articles, bool withEditableLinks = false)
        {
            _article.Contents.Should().NotBeNullOrEmpty();
            foreach (var article in articles)
            {
                var actual = _article.Contents.SingleOrDefault(x => x.Id == article.Id);
                actual.Language.Should().Be(article.Language);
                
                //TODO: Assert text from file
                //actual.Text.Should().Be(article.Text);
                
                actual.ArticleId.Should().Be(article.ArticleId);
                actual.Link(RelTypes.Self)
                    .ShouldBeGet()
                    .EndingWith($"libraries/{_libraryId}/articles/{_article.Id}/contents")
                    .ShouldHaveQueryParameter("language", actual.Language);

                if (withEditableLinks)
                {
                    actual.Link(RelTypes.Update)
                        .ShouldBePut()
                        .EndingWith($"libraries/{_libraryId}/articles/{_article.Id}/contents")
                        .ShouldHaveQueryParameter("language", actual.Language);

                    actual.Link(RelTypes.Delete)
                        .ShouldBeDelete()
                        .EndingWith($"libraries/{_libraryId}/articles/{_article.Id}/contents")
                        .ShouldHaveQueryParameter("language", actual.Language);
                }

            }
            return this;
        }

        public ArticleAssert ShouldHaveAddContentLink()
        {
            _article.Link("add-content")
                  .ShouldBePut()
                  .EndingWith($"libraries/{_libraryId}/articles/{_article.Id}/contents");
            return this;
        }

        public ArticleAssert ShouldNotHaveAddContentLink()
        {
            _article.Link("add-file").Should().BeNull();
            return this;
        }

        public void ShouldMatch(ArticleView view)
        {
            _article.Title.Should().Be(view.Title);
            _article.WriterAccountId.Should().Be(view.WriterAccountId);
            _article.WriterAccountName.Should().Be(view.WriterAccountName);
            if (view.WriterAssignTimeStamp.HasValue)
            {
                _article.WriterAssignTimeStamp.Should().BeCloseTo(view.WriterAssignTimeStamp.Value, TimeSpan.FromSeconds(2));
            }
            else
            {
                _article.WriterAssignTimeStamp.Should().Be(view.WriterAssignTimeStamp);
            }
            _article.ReviewerAccountId.Should().Be(view.ReviewerAccountId);
            if (view.ReviewerAssignTimeStamp.HasValue)
            {
                _article.ReviewerAssignTimeStamp.Should().BeCloseTo(view.ReviewerAssignTimeStamp.Value, TimeSpan.FromSeconds(2));
            }
            else
            {
                _article.ReviewerAssignTimeStamp.Should().Be(view.ReviewerAssignTimeStamp);
            }
            _article.ReviewerAccountName.Should().Be(view.ReviewerAccountName);
            _article.Status.Should().Be(view.Status);
        }

        public ArticleAssert ShouldMatch(ArticleDto dto)
        {
            _article.Title.Should().Be(dto.Title);
            _article.WriterAccountId.Should().Be(dto.WriterAccountId);
            if (dto.WriterAssignTimeStamp.HasValue)
            {
                _article.WriterAssignTimeStamp.Should().BeCloseTo(dto.WriterAssignTimeStamp.Value, TimeSpan.FromSeconds(2));
            }
            else
            {
                _article.WriterAssignTimeStamp.Should().Be(dto.WriterAssignTimeStamp);
            }

            _article.ReviewerAccountId.Should().Be(dto.ReviewerAccountId);
            if (dto.ReviewerAssignTimeStamp.HasValue)
            {
                _article.ReviewerAssignTimeStamp.Should().BeCloseTo(dto.ReviewerAssignTimeStamp.Value, TimeSpan.FromSeconds(2));
            }
            else
            {
                _article.ReviewerAssignTimeStamp.Should().Be(dto.ReviewerAssignTimeStamp);

            }

            _article.Status.Should().Be(dto.Status.ToString());
            return this;
        }

        public ArticleAssert ShouldBeSameAs(ArticleView _expected)
        {
            _article.Title.Should().Be(_expected.Title);
            _article.IsPublic.Should().Be(_expected.IsPublic);
            _article.WriterAccountId.Should().Be(_expected.WriterAccountId);
            if (_expected.WriterAssignTimeStamp.HasValue)
            {
                _article.WriterAssignTimeStamp.Should().BeCloseTo(_expected.WriterAssignTimeStamp.Value, TimeSpan.FromSeconds(2));
            }
            else
            {
                _article.WriterAssignTimeStamp.Should().BeNull();
            }
            _article.ReviewerAccountId.Should().Be(_expected.ReviewerAccountId);
            if (_expected.ReviewerAssignTimeStamp.HasValue)
            {
                _article.ReviewerAssignTimeStamp.Should().BeCloseTo(_expected.ReviewerAssignTimeStamp.Value, TimeSpan.FromSeconds(2));
            }
            else
            {
                _article.ReviewerAssignTimeStamp.Should().BeNull();
            }
            _article.Status.Should().Be(_expected.Status.ToString());

            var authors = _authorRepository.GetAuthorsByArticle(_expected.Id);
            _article.Authors.Should().HaveSameCount(authors);
            foreach (var author in authors)
            {
                var actual = _article.Authors.SingleOrDefault(a => a.Id == author.Id);
                actual.Name.Should().Be(author.Name);

                actual.Link("self")
                      .ShouldBeGet()
                      .EndingWith($"libraries/{_libraryId}/authors/{author.Id}");

                return this;
            }

            var categories = _categoryRepository.GetCategoriesByArticle(_expected.Id);
            _article.Authors.Should().HaveSameCount(categories);
            foreach (var category in categories)
            {
                var actual = _article.Categories.SingleOrDefault(a => a.Id == category.Id);
                actual.Name.Should().Be(category.Name);

                actual.Link("self")
                      .ShouldBeGet()
                      .EndingWith($"libraries/{_libraryId}/categories/{category.Id}");

                return this;
            }

            return this;
        }

        public ArticleAssert ShouldBeSameAs(ArticleDto _expected)
        {
            _article.Title.Should().Be(_expected.Title);
            _article.IsPublic.Should().Be(_expected.IsPublic);
            _article.Status.Should().Be(_expected.Status.ToString());
            _article.Type.Should().Be(_expected.Type.ToString());
            _article.LastModified.Should().BeCloseTo(_expected.LastModified, TimeSpan.FromSeconds(2));
            _article.WriterAccountId.Should().Be(_expected.WriterAccountId);
            if (_expected.WriterAssignTimeStamp.HasValue)
            {
                _article.WriterAssignTimeStamp.Should().BeCloseTo(_expected.WriterAssignTimeStamp.Value, TimeSpan.FromSeconds(2));
            }
            else
            {
                _article.WriterAssignTimeStamp.Should().BeNull();
            }
            _article.ReviewerAccountId.Should().Be(_expected.ReviewerAccountId);
            if (_expected.ReviewerAssignTimeStamp.HasValue)
            {
                _article.ReviewerAssignTimeStamp.Should().BeCloseTo(_expected.ReviewerAssignTimeStamp.Value, TimeSpan.FromSeconds(2));
            }
            else
            {
                _article.ReviewerAssignTimeStamp.Should().BeNull();
            }

            var authors = _authorRepository.GetAuthorsByArticle(_expected.Id);
            _article.Authors.Should().HaveSameCount(authors);
            foreach (var author in authors)
            {
                var actual = _article.Authors.SingleOrDefault(a => a.Id == author.Id);
                actual.Name.Should().Be(author.Name);

                actual.Link("self")
                      .ShouldBeGet()
                      .EndingWith($"libraries/{_libraryId}/authors/{author.Id}");

                return this;
            }

            var categories = _categoryRepository.GetCategoriesByArticle(_expected.Id);
            _article.Authors.Should().HaveSameCount(categories);
            foreach (var category in categories)
            {
                var actual = _article.Categories.SingleOrDefault(a => a.Id == category.Id);
                actual.Name.Should().Be(category.Name);

                actual.Link("self")
                      .ShouldBeGet()
                      .EndingWith($"libraries/{_libraryId}/categories/{category.Id}");

                return this;
            }

            return this;
        }

        public ArticleAssert ShouldBeSameCategories(IEnumerable<CategoryDto> categories)
        {
            foreach (var category in categories)
            {
                var actual = _article.Categories.SingleOrDefault(x => x.Id == category.Id);

                actual.Should().BeEquivalentTo(category, config => config.ExcludingMissingMembers());
            }
            return this;
        }

        public ArticleAssert ShouldHaveCategories(List<CategoryDto> categoriesToUpdate)
        {
            var dbCategories = _categoryRepository.GetCategoriesByArticle(_article.Id);
            dbCategories.Should().HaveSameCount(categoriesToUpdate);
            foreach (var category in categoriesToUpdate)
            {
                var actual = dbCategories.SingleOrDefault(x => x.Id == category.Id);

                actual.Should().BeEquivalentTo(category, config => config.ExcludingMissingMembers());
            }
            return this;
        }
    }
}
