using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class ArticleAssert
    {
        private HttpResponseMessage _response;
        private readonly int _libraryId;
        private ArticleView _article;

        public ArticleAssert(ArticleView view, int libraryId)
        {
            _libraryId = libraryId;
            _article = view;
        }

        public ArticleAssert(HttpResponseMessage response, int libraryId)
        {
            _response = response;
            _libraryId = libraryId;
            _article = response.GetContent<ArticleView>().Result;
        }

        internal static ArticleAssert FromResponse(HttpResponseMessage response, int libraryId)
        {
            return new ArticleAssert(response, libraryId);
        }

        internal static ArticleAssert FromObject(ArticleView view, int libraryId)
        {
            return new ArticleAssert(view, libraryId);
        }

        internal ArticleAssert ShouldHaveCorrectLocationHeader()
        {
            var location = _response.Headers.Location.AbsoluteUri;
            location.Should().NotBeNull();
            location.Should().EndWith($"libraries/{_libraryId}/articles/{_article.Id}");
            return this;
        }

        internal ArticleAssert ShouldBeAssignedToUserForWriting(AccountDto account)
        {
            _article.WriterAccountId.Should().Be(account.Id);
            _article.WriterAccountName.Should().Be(account.Name);
            _article.WriterAssignTimeStamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            return this;
        }

        internal ArticleAssert ShouldNotBeAssignedForWriting()
        {
            _article.WriterAccountId.Should().BeNull();
            _article.WriterAccountName.Should().BeNull();
            _article.WriterAssignTimeStamp.Should().BeNull();
            return this;
        }

        internal ArticleAssert ShouldBeSavedAssignmentForWriting(IDbConnection dbConnection, AccountDto account)
        {
            var dbArticle = dbConnection.GetArticleById(_article.Id);
            dbArticle.WriterAccountId.Should().Be(account.Id);
            dbArticle.WriterAssignTimeStamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            return this;
        }

        internal ArticleAssert ShouldBeSavedNoAssignmentForWriting(IDbConnection dbConnection)
        {
            var dbArticle = dbConnection.GetArticleById(_article.Id);
            dbArticle.WriterAccountId.Should().BeNull();
            dbArticle.WriterAssignTimeStamp.Should().BeNull();
            return this;
        }

        internal ArticleAssert ShouldBeAssignedToUserForReviewing(AccountDto account)
        {
            _article.ReviewerAccountId.Should().Be(account.Id);
            _article.ReviewerAccountName.Should().Be(account.Name);
            _article.ReviewerAssignTimeStamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            return this;
        }

        internal ArticleAssert ShouldNotBeAssignedForReviewing()
        {
            _article.ReviewerAccountId.Should().BeNull();
            _article.ReviewerAccountName.Should().BeNull();
            _article.ReviewerAssignTimeStamp.Should().BeNull();
            return this;
        }

        internal ArticleAssert ShouldBeSavedAssignmentForReviewing(IDbConnection dbConnection, AccountDto account)
        {
            var dbArticle = dbConnection.GetArticleById(_article.Id);
            dbArticle.ReviewerAccountId.Should().Be(account.Id);
            dbArticle.ReviewerAssignTimeStamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            return this;
        }

        internal ArticleAssert ShouldBeSavedNoAssignmentForReviewing(IDbConnection dbConnection)
        {
            var dbArticle = dbConnection.GetArticleById(_article.Id);
            dbArticle.ReviewerAccountId.Should().BeNull();
            dbArticle.ReviewerAssignTimeStamp.Should().BeNull();
            return this;
        }

        internal ArticleAssert ShouldHaveSavedArticle(IDbConnection dbConnection)
        {
            var dbArticle = dbConnection.GetArticleById(_article.Id);
            dbArticle.Should().NotBeNull();
            _article.Title.Should().Be(dbArticle.Title);
            return this;
        }

        internal static void ShouldHaveDeletedArticle(long articleId, IDbConnection databaseConnection)
        {
            var article = databaseConnection.GetArticleById(articleId);
            article.Should().BeNull();
        }

        internal static void ThatContentsAreDeletedForArticle(long articleId, IDbConnection databaseConnection)
        {
            var contents = databaseConnection.GetContentByArticle(articleId);
            contents.Should().BeNullOrEmpty();
        }

        public static void ShouldBeAddedToFavorite(long articleId, int accountId, IDbConnection dbConnection)
        {
            dbConnection.DoesArticleExistsInFavorites(articleId, accountId).Should().BeTrue();
        }

        public static void ShouldNotBeInFavorites(long articleId, int accountId, IDbConnection dbConnection)
        {
            dbConnection.DoesArticleExistsInFavorites(articleId, accountId).Should().BeFalse();
        }

        internal static void ShouldHaveDeletedArticleFromRecentReads(long articleId, IDbConnection dbConnection)
        {
            dbConnection.DoesArticleExistsInRecent(articleId).Should().BeFalse();
        }

        internal static void ShouldHaveDeletedArticleImage(long articleId, long imageId, string filename, IDbConnection databaseConnection, FakeFileStorage fileStorage)
        {
            var file = databaseConnection.GetFileById(imageId);
            file.Should().BeNull();
            fileStorage.DoesFileExists(filename);
        }

        internal static void ShouldHaveDeletedArticleContents(long articleId, IDbConnection databaseConnection)
        {
            var savedContents = databaseConnection.GetContentByArticle(articleId);
            savedContents.Should().BeNullOrEmpty();
        }

        internal static void ShouldNotHaveUpdatedArticleImage(long articleId, byte[] oldImage, IDbConnection dbConnection, IFileStorage fileStorage)
        {
            var imageUrl = dbConnection.GetArticleImageUrl(articleId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().Equal(oldImage);
        }

        internal static void ShouldHaveAddedArticleImage(long articleId, IDbConnection dbConnection, IFileStorage fileStorage)
        {
            var imageUrl = dbConnection.GetArticleImageUrl(articleId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNullOrEmpty();
        }

        internal static void ShouldHaveUpdatedArticleImage(long articleId, byte[] newImage, IDbConnection dbConnection, IFileStorage fileStorage)
        {
            var imageUrl = dbConnection.GetArticleImageUrl(articleId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNull().And.Equal(newImage);
        }

        internal ArticleAssert ShouldHaveSelfLink()
        {
            _article.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/articles/{_article.Id}");

            return this;
        }

        internal ArticleAssert WithReadOnlyLinks()
        {
            ShouldHaveSelfLink()
            .ShouldNotHaveAddArticleContentLink()
            .ShouldNotHaveUpdateLink()
            .ShouldNotHaveDeleteLink()
            .ShouldNotHaveAssignmentLink();
            return this;
        }

        internal ArticleAssert WithWriteableLinks()
        {
            ShouldHaveAddContentLink()
            .ShouldHaveUpdateLink()
            .ShouldHaveDeleteLink()
            .ShouldHaveAssignmentLink();
            return this;
        }

        internal void ShouldHaveNoCorrectContents()
        {
            _article.Link("content").Should().BeNull();
        }

        internal ArticleAssert ShouldHaveAssignmentLink()
        {
            _article.Link("assign")
                  .ShouldBePost()
                  .EndingWith($"libraries/{_libraryId}/articles/{_article.Id}/assign");
            return this;
        }

        internal ArticleAssert ShouldNotHaveAssignmentLink()
        {
            _article.Link("assign")
                  .Should().BeNull();
            return this;
        }

        internal ArticleAssert ShouldHaveUpdateLink()
        {
            _article.UpdateLink()
                 .ShouldBePut()
                 .EndingWith($"libraries/{_libraryId}/articles/{_article.Id}");

            return this;
        }

        internal ArticleAssert ShouldNotHaveUpdateLink()
        {
            _article.UpdateLink().Should().BeNull();
            return this;
        }

        internal ArticleAssert ShouldHaveDeleteLink()
        {
            _article.DeleteLink()
                 .ShouldBeDelete()
                 .EndingWith($"libraries/{_libraryId}/articles/{_article.Id}");

            return this;
        }

        internal ArticleAssert ShouldNotHaveDeleteLink()
        {
            _article.DeleteLink().Should().BeNull();
            return this;
        }

        internal ArticleAssert ShouldNotHaveAddArticleContentLink()
        {
            _article.Link("add-content").Should().BeNull();
            return this;
        }

        internal ArticleAssert ShouldHaveUpdateContentLink(IssueContentDto content)
        {
            var actual = _article.Contents.Single(x => x.Id == content.Id);
            actual.UpdateLink()
                  .ShouldBePut()
                  .EndingWith($"libraries/{_libraryId}/articles/{_article.Id}")
                  .ShouldHaveAcceptLanguage(content.Language);

            return this;
        }

        internal ArticleAssert ShouldHaveDeleteContentLink(IssueContentDto content)
        {
            var actual = _article.Contents.Single(x => x.Id == content.Id);
            actual.DeleteLink()
                  .ShouldBeDelete()
                  .EndingWith($"libraries/{_libraryId}/articles/{_article.Id}")
                  .ShouldHaveAcceptLanguage(actual.Language);

            return this;
        }

        internal ArticleAssert ShouldNotHaveContentsLink()
        {
            _article.Link("content").Should().BeNull();
            return this;
        }

        internal ArticleAssert ShouldHavePublicImageLink()
        {
            _article.Link("image")
                .ShouldBeGet();
            //.Href.Should().StartWith(Settings.CDNAddress);

            return this;
        }

        internal ArticleAssert ShouldNotHaveImageUpdateLink()
        {
            _article.Link("image-upload").Should().BeNull();
            return this;
        }

        internal ArticleAssert ShouldHaveImageUpdateLink()
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

        internal ArticleAssert ShouldHaveCorrectImageLocationHeader(long articleId)
        {
            _response.Headers.Location.AbsoluteUri.Should().NotBeEmpty();
            return this;
        }

        internal static void ShouldHavePublicImage(long articleId, IDbConnection dbConnection)
        {
            var image = dbConnection.GetArticleImage(articleId);
            image.Should().NotBeNull();
            image.IsPublic.Should().BeTrue();
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

        internal void ShouldMatch(ArticleView view)
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

        internal void ShouldMatch(ArticleDto dto)
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
        }

        internal ArticleAssert ShouldBeSameAs(ArticleView _expected, IDbConnection db)
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

            var authors = db.GetAuthorsByArticle(_expected.Id);
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

            var categories = db.GetCategoriesByArticle(_expected.Id);
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

        internal ArticleAssert ShouldBeSameAs(ArticleDto _expected, IDbConnection db)
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

            var authors = db.GetAuthorsByArticle(_expected.Id);
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

            var categories = db.GetCategoriesByArticle(_expected.Id);
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

        internal ArticleAssert ShouldBeSameCategories(IEnumerable<CategoryDto> categories)
        {
            foreach (var category in categories)
            {
                var actual = _article.Categories.SingleOrDefault(x => x.Id == category.Id);

                actual.Should().BeEquivalentTo(category, config => config.ExcludingMissingMembers());
            }
            return this;
        }

        internal ArticleAssert ShouldHaveCategories(List<CategoryDto> categoriesToUpdate, IDbConnection databaseConnection)
        {
            var dbCategories = databaseConnection.GetCategoriesByArticle(_article.Id);
            dbCategories.Should().HaveSameCount(categoriesToUpdate);
            foreach (var category in categoriesToUpdate)
            {
                var actual = dbCategories.SingleOrDefault(x => x.Id == category.Id);

                actual.Should().BeEquivalentTo(category, config => config.ExcludingMissingMembers());
            }
            return this;
        }
    }


    public static class ArticleAssertionExtensions
    {
        public static ArticleAssert ShouldMatch(this ArticleView view, ArticleDto dto, IDbConnection dbConnection, int libraryId)
        {
            return new ArticleAssert(view, libraryId)
                               .ShouldBeSameAs(dto, dbConnection);
        }
    }
}
