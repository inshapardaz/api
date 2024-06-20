using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Articles.UpdateArticleContent
{
    [TestFixture]
    public class WhenUpdatingArticleContentsWithDifferentLanguage
        : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleDto _article;
        private ArticleContentDto _content;
        private ArticleContentAssert _assert;

        private string _newContents, _newLayout, _newLanguge;

        public WhenUpdatingArticleContentsWithDifferentLanguage()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _article = ArticleBuilder.WithLibrary(LibraryId).IsPublic().WithContent().WithContentLanguage("de").Build();
            _content = ArticleBuilder.Contents.Single(x => x.ArticleId == _article.Id);

            _newContents = RandomData.String;
            _newLayout = RandomData.String;
            _newLanguge = "en";

            _response = await Client.PutObject($"/libraries/{LibraryId}/articles/{_article.Id}/contents",
                new ArticleContentView
                {
                    Text = _newContents,
                    Language = _newLanguge,
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
        public void ShouldHaveCreatedCorrectContents()
        {
            _assert.ShouldHaveMatechingTextForLanguage(_newContents, _newLanguge, _newLayout);
        }

        [Test]
        public void ShouldHaveOtherLanguageContents()
        {
            _assert.ShouldHaveContent(_article.Id, _content.Language);
        }
    }
}
