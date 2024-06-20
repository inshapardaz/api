using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
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
        private string _newContents, _newLayout;

        public WhenUpdatingArticleContentsWhereContentNotPresent()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _article = ArticleBuilder.WithLibrary(LibraryId).IsPublic().Build();

            _newContents = RandomData.String;
            _newLayout = RandomData.String;

            _response = await Client.PutObject($"/libraries/{LibraryId}/articles/{_article.Id}/contents",
                new ArticleContentView
                {
                    Text = _newContents,
                    Language = _language,
                    Layout = _newLayout
                });

            _assert = Services.GetService<ArticleContentAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
        public void ShouldSaveTheArticleContent()
        {
            _assert.ShouldHaveSavedArticleContent();
        }

        [Test]
        public void ShouldHaveTextReturened()
        {
            _assert.ShouldHaveText(_newContents);
        }

        [Test]
        public void ShouldHaveCorrectContentSaved()
        {
            _assert.ShouldHaveSavedCorrectText(_newContents);
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
