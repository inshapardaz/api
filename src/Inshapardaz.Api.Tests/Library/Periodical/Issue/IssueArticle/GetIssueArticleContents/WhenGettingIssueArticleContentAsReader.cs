﻿using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Tests.Dto;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.IssueArticle.GetIssueArticleContents
{
    [TestFixture]
    public class WhenGettingIssueArticleContentAsReader
        : TestBase
    {
        private HttpResponseMessage _response;
        private IssueArticleContentAssert _assert;
        private IssueDto _issue;
        private IssueArticleDto _article;
        private IssueArticleContentDto _content;

        public WhenGettingIssueArticleContentAsReader()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _issue = IssueBuilder.WithLibrary(LibraryId).WithArticles(1).WithArticleContents(4).Build();
            _article = IssueBuilder.GetArticles(_issue.Id).PickRandom();
            _content = IssueBuilder.ArticleContents.Where(x => x.ArticleId == _article.Id).PickRandom();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/{_article.SequenceNumber}/contents", _content.Language);
            _assert = new IssueArticleContentAssert(_response, Library, _issue);
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
        public void ShouldHavePeriodicalLink()
        {
            _assert.ShouldHavePeriodicalLink();
        }

        [Test]
        public void ShouldHaveIssueLink()
        {
            _assert.ShouldHaveIssueLink();
        }


        [Test]
        public void ShouldHaveArticleLink()
        {
            _assert.ShouldHaveArticleLink();
        }

        [Test]
        public void ShouldNotHaveEditLinks()
        {
            _assert.ShouldNotHaveUpdateLink()
                .ShouldNotHaveDeleteLink();
        }


        [Test]
        public void ShouldHaveTextReturened()
        {
            _assert.ShouldHaveText(_content.Text);
        }

        [Test]
        public void ShouldReturnCorrectArticleData()
        {
            _assert.ShouldMatch(_content, _issue, _article);
        }
    }
}
