using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.User.UpsertUserIssueRating
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.Reader)]
    public class WhenAddingUserIssueRatingWithPermissions(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;
        private IssueRatingAssert _assert;
        private IssueDto _issue;
        private RatingView _payload;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _issue = IssueBuilder.WithLibrary(LibraryId).Build();

            _payload = new RatingView
            {
                Value = 4
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/my/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/rating", _payload);
            _assert = Services.GetService<IssueRatingAssert>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveOkResult() => _response.ShouldBeOk();

        [Test]
        public void ShouldReturnSavedRating() => _assert.ShouldMatch(_payload.Value);

        [Test]
        public void ShouldHaveSavedRating() => _assert.ShouldHaveSaved(_issue.Id, AccountId, _payload.Value)
            .ShouldHaveBeenAdded(_issue.Id, AccountId);
    }
}
