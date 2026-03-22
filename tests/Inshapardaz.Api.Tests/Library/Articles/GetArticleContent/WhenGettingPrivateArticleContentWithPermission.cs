using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.GetArticleContent
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenGettingPrivateArticleContentWithPermission(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;
        private ArticleContentAssert _assert;
        private ArticleDto _article;
        private ArticleContentDto _content;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _article = ArticleBuilder.WithLibrary(LibraryId).IsPublic(false).WithContent().Build();
            _content = ArticleBuilder.Contents.Single(x => x.ArticleId == _article.Id);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/articles/{_article.Id}/contents?language={_content.Language}");
            _assert = Services.GetService<ArticleContentAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldHaveSelfLink() => _assert.ShouldHaveSelfLink();

        [Test]
        public void ShouldHaveArticleLink() => _assert.ShouldHaveArticleLink();

        [Test]
        public void ShouldHaveTextReturened() => _assert.ShouldHaveText(_content);

        [Test]
        public void ShouldHaveEditLinks()
        {
            _assert.ShouldHaveUpdateLink();
            _assert.ShouldHaveDeleteLink();
        }

        [Test]
        public void ShouldReturnCorrectArticleData() => _assert.ShouldMatch(_content, _article);
    }
}
