using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.DeleteUserIssueRating
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.Reader)]
    public class WhenDeletingUserIssueRatingWithPermissions(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;
        private IssueRatingAssert _assert;
        private IssueDto _issue;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _issue = IssueBuilder.WithLibrary(LibraryId).Build();

            IssueTestRepository.AddRating(new IssueRatingDto
            {
                IssueId = _issue.Id,
                LibraryId = LibraryId,
                AccountId = AccountId,
                Value = 3,
                DateAdded = DateTime.UtcNow
            });

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/my/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/rating");
            _assert = Services.GetService<IssueRatingAssert>();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveNoContentResult() => _response.ShouldBeNoContent();

        [Test]
        public void ShouldHaveDeletedRating() => _assert.ShouldNotExist(_issue.Id, AccountId);
    }
}
