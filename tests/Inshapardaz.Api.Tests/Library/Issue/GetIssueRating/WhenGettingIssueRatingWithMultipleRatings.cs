using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Issue.GetIssueRating
{
    [TestFixture]
    public class WhenGettingIssueRatingWithMultipleRatings() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;
        private IssueRatingAssert _assert;
        private IssueDto _issue;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _issue = IssueBuilder.WithLibrary(LibraryId).Build();

            var firstAccount = AccountBuilder.As(Role.Reader).Build();
            var secondAccount = AccountBuilder.As(Role.Reader).Build();

            IssueTestRepository.AddRating(new IssueRatingDto { IssueId = _issue.Id, LibraryId = LibraryId, AccountId = AccountId, Value = 5, DateAdded = DateTime.UtcNow });
            IssueTestRepository.AddRating(new IssueRatingDto { IssueId = _issue.Id, LibraryId = LibraryId, AccountId = firstAccount.Id, Value = 3, DateAdded = DateTime.UtcNow });
            IssueTestRepository.AddRating(new IssueRatingDto { IssueId = _issue.Id, LibraryId = LibraryId, AccountId = secondAccount.Id, Value = 4, DateAdded = DateTime.UtcNow });

            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/rating");
            _assert = Services.GetService<IssueRatingAssert>().ForSummaryResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveOkResult() => _response.ShouldBeOk();

        [Test]
        public void ShouldHaveAverageOfAllRatings() => _assert.ShouldHaveAverage(4.0, 3);
    }
}
