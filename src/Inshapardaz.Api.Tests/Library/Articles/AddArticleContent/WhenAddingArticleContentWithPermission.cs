using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Articles.AddArticleContent
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenAddingArticleContentWithPermission
        : TestBase
    {
        private HttpResponseMessage _response;
        private string _locale;
        private ArticleContentAssert _assert;
        private string _contents;

        public WhenAddingArticleContentWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _locale = RandomData.Locale;

            var article = ArticleBuilder.WithLibrary(LibraryId).Build();
            _contents = RandomData.Text;

            _response = await Client.PostString($"/libraries/{LibraryId}/articles/{article.Id}/contents", _contents, _locale);

            _assert = new ArticleContentAssert(_response, LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveCorrectLink()
        {
            _assert.ShouldHaveSelfLink()
                   .ShouldHaveArticleLink()
                   .ShouldHaveUpdateLink()
                   .ShouldHaveDeleteLink();
        }

        [Test]
        public void ShouldHaceCorrectContentSaved()
        {
            _assert.ShouldHaveSavedArticleContent(DatabaseConnection);
        }
    }
}
