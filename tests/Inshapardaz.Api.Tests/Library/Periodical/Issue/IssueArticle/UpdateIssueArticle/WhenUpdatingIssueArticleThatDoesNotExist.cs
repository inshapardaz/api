﻿using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.IssueArticle.UpdateIssueArticle
{
    [TestFixture]
    public class WhenUpdatingIssueArticleThatDoesNotExist
        : TestBase
    {
        private HttpResponseMessage _response;
        private IssueArticleAssert _articleAssert;
        private IssueArticleView _newArticle;

        public WhenUpdatingIssueArticleThatDoesNotExist()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithArticles(5).Build();

            _newArticle = new IssueArticleView { Title = RandomData.Name, SequenceNumber = 10 };

            _response = await Client.PutObject($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/articles/{_newArticle.SequenceNumber + 1}", _newArticle);

            _articleAssert = Services.GetService<IssueArticleAssert>().ForResponse(_response)
                    .ForLibrary(LibraryId)
                    .ForIssueDto(issue);
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
            _articleAssert.ShouldHaveSavedArticle();
        }

        [Test]
        public void ShouldHaveCorrectObjectReturned()
        {
            IssueArticleView expected = new IssueArticleView
            {
                Title = _newArticle.Title,
                SequenceNumber = 6,
                Status = "Available"
            };
            _articleAssert.ShouldMatch(expected);
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
