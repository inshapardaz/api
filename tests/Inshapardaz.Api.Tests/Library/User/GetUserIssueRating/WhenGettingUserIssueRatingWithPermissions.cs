using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.GetUserIssueRating
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.Reader)]
    public class WhenGettingUserIssueRatingWithPermissions(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;
        private IssueRatingAssert _assert;
        private IssueDto _issue;
        private IssueRatingDto _rating;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _issue = IssueBuilder.WithLibrary(LibraryId).Build();

            _rating = new IssueRatingDto
            {
                IssueId = _issue.Id,
                LibraryId = LibraryId,
                AccountId = AccountId,
                Value = 3,
                DateAdded = DateTime.UtcNow
            };
            IssueTestRepository.AddRating(_rating);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/my/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/rating");
            _assert = Services.GetService<IssueRatingAssert>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveOkResult() => _response.ShouldBeOk();

        [Test]
        public void ShouldHaveExpectedRating() => _assert.ShouldMatch(_rating.Value);
    }
}
