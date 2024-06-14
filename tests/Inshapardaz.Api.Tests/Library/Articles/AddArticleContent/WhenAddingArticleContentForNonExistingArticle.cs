using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Articles.AddArticleContent
{
    [TestFixture]
    public class WhenAddingArticleContentForNonExistingArticle
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAddingArticleContentForNonExistingArticle()
            : base(Role.LibraryAdmin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.PostObject($"/libraries/{LibraryId}/articles/{-RandomData.Number}/contents",
                new ArticleContentView
                {
                    Text = RandomData.Text,
                    Language = "hi",
                    Layout = RandomData.String
                });
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveBadRequestResult()
        {
            _response.ShouldBeBadRequest();
        }
    }
}
