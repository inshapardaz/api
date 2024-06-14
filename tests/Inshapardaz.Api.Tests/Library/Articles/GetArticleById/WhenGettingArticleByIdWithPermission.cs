using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.GetArticleById
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenGettingArticleByIdWithPermission : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleDto _expected;
        private ArticleAssert _assert;
        private IEnumerable<CategoryDto> _categories;

        public WhenGettingArticleByIdWithPermission(Role role) : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _categories = CategoryBuilder.WithLibrary(LibraryId).Build(2);
            var articles = ArticleBuilder.WithLibrary(LibraryId)
                                        .WithCategories(_categories)
                                        .WithContent()
                                        .Build(4);
            _expected = articles.PickRandom();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/articles/{_expected.Id}");
            _assert = ArticleAssert.FromResponse(_response, LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _assert.ShouldHaveSelfLink();
        }

        [Test]
        public void ShouldHaveContents()
        {
            _assert.ShouldHaveContents(ArticleBuilder.Contents.Where(x => x.ArticleId == _expected.Id).ToList(), true);
        }

        [Test]
        public void ShouldHaveImageLink()
        {
            _assert.ShouldHavePublicImageLink();
        }

        [Test]
        public void ShouldHaveUpdateLink()
        {
            _assert.ShouldHaveUpdateLink();
        }

        [Test]
        public void ShouldHaveDeleteLink()
        {
            _assert.ShouldHaveDeleteLink();
        }

        [Test]
        public void ShouldHaveAddContentLink()
        {
            _assert.ShouldHaveAddContentLink();
        }

        [Test]
        public void ShouldHaveAddFavoriteLinks()
        {
            _assert.ShouldHaveAddFavoriteLink();
        }

        [Test]
        public void ShouldReturnCorrectArticleData()
        {
            _assert.ShouldBeSameAs(_expected, DatabaseConnection);
        }
    }
}
