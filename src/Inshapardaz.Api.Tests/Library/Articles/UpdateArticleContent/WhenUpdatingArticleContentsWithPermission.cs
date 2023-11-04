using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Articles.UpdateArticleContent
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenUpdatingArticleContentsWithPermission
        : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleDto _article;
        private ArticleContentDto _content;
        private ArticleContentAssert _assert;

        private string _newContents;

        public WhenUpdatingArticleContentsWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _article = ArticleBuilder.WithLibrary(LibraryId).IsPublic().WithContent().Build();
            _content = ArticleBuilder.Contents.Single(x => x.ArticleId == _article.Id);

            _newContents = RandomData.String;

            _response = await Client.PutString($"/libraries/{LibraryId}/articles/{_article.Id}/contents?language={_content.Language}", _newContents);
            _assert = new ArticleContentAssert(_response, LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                   .ShouldHaveArticleLink()
                   .ShouldHaveUpdateLink()
                   .ShouldHaveDeleteLink();
        }

        [Test]
        public void ShouldHaveTextReturened()
        {
            _assert.ShouldHaveText(_newContents);
        }

        [Test]
        public void ShouldHaveUpdatedContents()
        {
            _assert.ShouldHaveSavedCorrectText(_newContents, DatabaseConnection);
        }
    }
}
