using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
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
        private string _locale, _contents, _layout;
        private ArticleContentAssert _assert;

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
            _layout = RandomData.String;

            _response = await Client.PostObject($"/libraries/{LibraryId}/articles/{article.Id}/contents",
                new ArticleContentView
                {
                    Text = _contents,
                    Language = _locale,
                    Layout = _layout
                });

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
            _assert.ShouldHaveSavedArticleContent(DatabaseConnection, FileStore);
        }
    }
}
