using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.IssueArticle.UpdateIssueArticleSequence
{
    [TestFixture]
    public class WhenUpdatingIssueArticleSequenceThatDoesNotExist
        : TestBase
    {
        private HttpResponseMessage _response;
        private IEnumerable<IssueArticleDto> _articles;
        private IssueDto _issue;

        public WhenUpdatingIssueArticleSequenceThatDoesNotExist()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _issue = IssueBuilder.WithLibrary(LibraryId).WithArticles(5).Build();
            _articles = IssueBuilder.GetArticles(_issue.Id);

            var newSequence = _articles
                .OrderByDescending(x => x.SequenceNumber)
                .Select((x, i) => new SequenceView
                {
                    Id = x.Id,
                    SequenceNumber = i 
                }).ToList();

            newSequence.Last().Id = -RandomData.Number;

            _response = await Client.PostObject($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/sequence", newSequence);
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

        [Test]
        public void ShouldNotHaveUpdatedTheSequence()
        {
            var articles = IssueArticleTestRepository.GetIssueArticlesByIssue(_issue.Id);
            foreach (var articleDto in _articles)
            {
                var dbArticle = articles.SingleOrDefault(x => x.Id == articleDto.Id);
                articleDto.SequenceNumber.Should().Be(dbArticle.SequenceNumber);
            }
        }
    }
}
