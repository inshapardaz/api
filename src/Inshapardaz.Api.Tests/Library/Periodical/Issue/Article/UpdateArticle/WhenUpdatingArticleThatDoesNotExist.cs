﻿using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.Article.UpdateArticle
{
    [TestFixture]
    public class WhenUpdatingArticleThatDoesNotExist
        : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleAssert _articleAssert;
        private ArticleView _newArticle;

        public WhenUpdatingArticleThatDoesNotExist()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithArticles(5).Build();

            _newArticle = new ArticleView { Title = RandomData.Name, SequenceNumber = 10 };

            _response = await Client.PutObject($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/articles/{_newArticle.SequenceNumber + 1}", _newArticle);

            _articleAssert = ArticleAssert.FromResponse(_response, LibraryId, issue);
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
            _articleAssert.ShouldHaveCorrectLocationHeader();
        }

        [Test]
        public void ShouldSaveTheIssue()
        {
            _articleAssert.ShouldHaveSavedArticle(DatabaseConnection);
        }

        [Test]
        public void ShouldHaveCorrectObjectRetured()
        {
            _articleAssert.ShouldMatch(_newArticle);
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _articleAssert.ShouldHaveSelfLink()
                   .ShouldHavePeriodicalLink()
                   .ShouldHaveIssueLink()
                   .ShouldHaveUpdateLink()
                   .ShouldHaveDeleteLink()
                   .ShouldNotHaveContentsLink()
                   .ShouldHaveAddIssueContentLink();
        }
    }
}