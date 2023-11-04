using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Articles.UpdateArticleContent
{
    [TestFixture]
    public class WhenUpdatingArticleContentsWhereContentNotPresent
        : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleDto _article;
        private ArticleContentAssert _assert;
        private string _language = RandomData.Locale;
        private string _newContents;

        public WhenUpdatingArticleContentsWhereContentNotPresent()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _article = ArticleBuilder.WithLibrary(LibraryId).IsPublic().Build();

            _newContents = RandomData.String;

            _response = await Client.PutString($"/libraries/{LibraryId}/articles/{_article.Id}/contents?language={_language}", _newContents);

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
        public void ShouldHaveLocationHeader()
        {
            _assert.ShouldHaveCorrectLocationHeader(_language);
        }

        [Test]
        public void ShouldSaveTheChapterContent()
        {
            _assert.ShouldHaveSavedArticleContent(DatabaseConnection);
        }

        [Test]
        public void ShouldHaveTextReturened()
        {
            _assert.ShouldHaveText(_newContents);
        }

        [Test]
        public void ShouldHaveCorrectContentSaved()
        {
            _assert.ShouldHaveSavedCorrectText(_newContents, DatabaseConnection);
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                   .ShouldHaveArticleLink()
                   .ShouldHaveUpdateLink()
                   .ShouldHaveDeleteLink();
        }
    }
}
