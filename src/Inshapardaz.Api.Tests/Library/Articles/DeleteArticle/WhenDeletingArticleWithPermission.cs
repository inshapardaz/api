using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.DeleteArticle
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingArticleWithPermission : TestBase
    {
        private HttpResponseMessage _response;

        private ArticleDto _expected;

        private int _authorId;

        public WhenDeletingArticleWithPermission(Role role) : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _expected = ArticleBuilder.WithLibrary(LibraryId)
                                    .WithCategories(1)
                                    .AddToFavorites(AccountId)
                                    .AddToRecentReads(AccountId)
                                    .Build(1)
                                    .Single();

            _authorId = ArticleBuilder.Authors.First().Id;

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/articles/{_expected.Id}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            _response.ShouldBeNoContent();
        }

        [Test]
        public void ShouldHaveDeletedArticle()
        {
            ArticleAssert.ShouldHaveDeletedArticle(_expected.Id, DatabaseConnection);
        }

        [Test]
        public void ShouldNotHaveDeletedTheAuthor()
        {
            AuthorAssert.ShouldNotHaveDeletedAuthor(_authorId, DatabaseConnection);
        }

        [Test]
        public void ShouldNotHaveDeletedTheCategory()
        {
            var cats = DatabaseConnection.GetCategoriesByArticle(_expected.Id);
            cats.ForEach(cat => CategoryAssert.ShouldNotHaveDeletedCategory(LibraryId, cat.Id, DatabaseConnection));
        }

        [Test]
        public void ShouldBeDeletedFromTheFavoritesOfAllUsers()
        {
            ArticleAssert.ShouldNotBeInFavorites(_expected.Id, AccountId, DatabaseConnection);
        }

        [Test]
        public void ShouldBeDeletedFromTheRecentReadArticles()
        {
            ArticleAssert.ShouldHaveDeletedBookFromRecentReads(_expected.Id, DatabaseConnection);
        }
    }
}
