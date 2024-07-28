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
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.IssueArticle.UpdateIssueArticleSequence
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenUpdatingIssueArticleSequenceWithPermission
        : TestBase
    {
        private HttpResponseMessage _response;
        private List<SequenceView> _newSequence;
        private IssueDto _issue;

        public WhenUpdatingIssueArticleSequenceWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _issue = IssueBuilder.WithLibrary(LibraryId).WithArticles(5).Build();
            var articles = IssueBuilder.GetArticles(_issue.Id);

            _newSequence = articles
                .OrderByDescending(x => x.SequenceNumber)
                .Select((x, i) => new SequenceView
                {
                    Id = x.Id,
                    SequenceNumber = i 
                }).ToList();

            _response = await Client.PostObject($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/articles/sequence", _newSequence);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOKResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldNotHaveUpdatedTheSequence()
        {
            var articles = IssueArticleTestRepository.GetIssueArticlesByIssue(_issue.Id);
            foreach (var articleDto in articles)
            {
                var article = _newSequence.SingleOrDefault(x => x.Id == articleDto.Id);
                articleDto.SequenceNumber.Should().Be(article.SequenceNumber);
            }
        }
    }
}
