﻿using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Articles.GetArticleContent
{
    [TestFixture]
    public class WhenGettingArticleContentWithoutLanguage
        : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleContentAssert _assert;
        private ArticleDto _article;
        private ArticleContentDto _content;

        public WhenGettingArticleContentWithoutLanguage()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _article = ArticleBuilder.WithLibrary(LibraryId).WithContent().WithContentLanguage(Library.Language).Build();
            _content = ArticleBuilder.Contents.Single(x => x.ArticleId == _article.Id);
            
            _response = await Client.GetAsync($"/libraries/{LibraryId}/articles/{_article.Id}/contents");
            _assert = Services.GetService<ArticleContentAssert>().ForLibrary(Library).ForResponse(_response);
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
        public void ShouldReturnCorrectLanguage()
        {
            _assert.ShouldHaveDefaultLibraryLanguage();
        }

        [Test]
        public void ShouldHaveTextReturened()
        {
            _assert.ShouldHaveText(_content);
        }

        [Test]
        public void ShouldReturnCorrectArticleData()
        {
            _assert.ShouldMatch(_content, _article);
        }
    }
}
