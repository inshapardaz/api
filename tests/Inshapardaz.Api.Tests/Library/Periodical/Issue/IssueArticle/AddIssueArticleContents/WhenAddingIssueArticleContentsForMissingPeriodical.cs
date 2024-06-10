﻿using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.IssueArticle.AddIssueArticleContents
{
    [TestFixture]
    public class WhenAddingIssueArticleContentsForMissingPeriodical
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAddingIssueArticleContentsForMissingPeriodical()
            : base(Role.LibraryAdmin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithArticles(5).Build();
            var articles = IssueBuilder.GetArticles(issue.Id);
            var article = RandomData.PickRandom(articles);

            _response = await Client.PostString($"/libraries/{LibraryId}/periodicals/{-issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/articles/{article.SequenceNumber}/contents?language={RandomData.Locale}", RandomData.String);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveBadRequestResult()
        {
            _response.ShouldBeBadRequest();
        }
    }
}