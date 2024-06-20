using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.GetArticleById
{
    [TestFixture]
    public class WhenGettingArticleByIdAsAnonymous : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleDto _expected;
        private IEnumerable<CategoryDto> _categories;

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
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnunUnauthorized()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
