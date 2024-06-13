using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.DataBuilders;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using PDFiumSharp.Types;

namespace Inshapardaz.Api.Tests.Library.Articles.DeleteArticle
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingArticleWithPermission : TestBase
    {
        private HttpResponseMessage _response;

        private ArticleDto _expected;
        private IEnumerable<ArticleContentDto> _contents;
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
                                    .WithContents(2)
                                    .Build(1)
                                    .Single();

            _contents = DatabaseConnection.GetArticleContents(_expected.Id);
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
            ArticleAssert.ShouldHaveDeletedArticleFromRecentReads(_expected.Id, DatabaseConnection);
        }

        [Test]
        public void ShouldHaveDeletedTheArticleImage()
        {
            var imageFile = ArticleBuilder.Files.Single(x => x.Id == _expected.ImageId);
            FileAssert.FileDoesnotExist(imageFile);
        }

        [Test]
        public void ShouldHaveDeletedArticleContents()
        {
            ArticleAssert.ShouldHaveDeletedArticleContents(_expected.Id, DatabaseConnection);
            ArticleBuilder.Files.Where(x => _contents.Any(y => y.FileId == x.Id))
                .ForEach(x => FileAssert.FileDoesnotExist(x));
        }
    }
}
