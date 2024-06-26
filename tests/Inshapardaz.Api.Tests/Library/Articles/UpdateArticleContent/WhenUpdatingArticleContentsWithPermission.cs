﻿using Inshapardaz.Api.Tests.Framework.Asserts;
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

        private string _newContents, _newLayout;

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
            _newLayout = RandomData.String;

            _response = await Client.PutObject($"/libraries/{LibraryId}/articles/{_article.Id}/contents",
                new ArticleContentView
                {
                    Text = _newContents,
                    Language = _content.Language,
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
            _assert.ShouldHaveSavedCorrectText(_newContents);
        }
    }
}
