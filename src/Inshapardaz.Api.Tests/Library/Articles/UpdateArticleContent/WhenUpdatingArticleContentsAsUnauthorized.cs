﻿using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Articles.UpdateArticleContent
{
    [TestFixture]
    public class WhenUpdatingArticleContentsAsUnauthorised
        : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleDto _article;
        private ArticleContentDto _content;

        private string _newContents;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _article = ArticleBuilder.WithLibrary(LibraryId).IsPublic().WithContent().Build();
            _content = ArticleBuilder.Contents.Single(x => x.ArticleId == _article.Id);

            _newContents = RandomData.String;

            _response = await Client.PutString($"/libraries/{LibraryId}/articles/{_article.Id}/contents?language={_content.Language}", _newContents);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorizedResult()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
