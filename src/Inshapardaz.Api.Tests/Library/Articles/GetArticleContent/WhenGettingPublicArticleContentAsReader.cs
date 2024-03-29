﻿using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Articles.GetArticleContent
{
    [TestFixture]
    public class WhenGettingPublicArticleContentAsReader
        : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleContentAssert _assert;
        private ArticleDto _article;
        private ArticleContentDto _content;

        public WhenGettingPublicArticleContentAsReader()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _article = ArticleBuilder.WithLibrary(LibraryId).IsPublic().WithContent().Build();
            _content = ArticleBuilder.Contents.Single(x => x.ArticleId == _article.Id);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/articles/{_article.Id}/contents?language={_content.Language}");
            _assert = new ArticleContentAssert(_response, LibraryId);
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
        public void ShouldHaveArticleLink()
        {
            _assert.ShouldHaveArticleLink();
        }

        [Test]
        public void ShouldHaveTextReturened()
        {
            _assert.ShouldHaveText(_content.Text);
        }

        [Test]
        public void ShouldReturnCorrectArticleData()
        {
            _assert.ShouldMatch(_content, _article);
        }
    }
}
