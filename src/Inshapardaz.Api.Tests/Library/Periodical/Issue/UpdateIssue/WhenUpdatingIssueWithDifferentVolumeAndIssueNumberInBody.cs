using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.UpdateIssue
{
    public class WhenUpdatingIssueWithDifferentVolumeAndIssueNumberInBody
        : TestBase
    {
        private HttpResponseMessage _response;
        private IssueView _newIssue;
        private IssueDto _oldIssue;
        private IssueAssert _assert;

        public WhenUpdatingIssueWithDifferentVolumeAndIssueNumberInBody()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issues = IssueBuilder.WithLibrary(LibraryId).Build(4);
            _oldIssue = issues.PickRandom();

            _newIssue = new IssueView { IssueDate = RandomData.Date, VolumeNumber = _oldIssue.VolumeNumber + 3, IssueNumber = _oldIssue.IssueNumber + 5 };

            _response = await Client.PutObject($"/libraries/{LibraryId}/periodicals/{_oldIssue.PeriodicalId}/volumes/{_oldIssue.VolumeNumber}/issues/{_oldIssue.IssueNumber}", _newIssue); _response = await Client.PutObject($"/libraries/{LibraryId}/periodicals/{_oldIssue.PeriodicalId}/volumes/{_oldIssue.VolumeNumber}/issues/{_oldIssue.IssueNumber}", _newIssue);
            _assert = IssueAssert.FromResponse(_response).InLibrary(LibraryId);
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
        public void ShouldHaveReturnedUpdatedChapter()
        {
            var issue = new IssueDto()
            {
                PeriodicalId = _oldIssue.PeriodicalId,
                VolumeNumber = _oldIssue.VolumeNumber,
                IssueNumber = _oldIssue.IssueNumber,
                IssueDate = _newIssue.IssueDate
                
            };
            
            _assert.ShouldBeSameAs(DatabaseConnection, issue);
        }

        [Test]
        public void ShouldHaveUpdatedChater()
        {
            _assert.ShouldHaveSavedIssue(DatabaseConnection);
        }
    }
}
