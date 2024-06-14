using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.IssuePage.UpdatePage
{
    [TestFixture]
    public class WhenUpdatingIssuePageWhenPageDoesNotExist : TestBase
    {
        private HttpResponseMessage _response;
        private IssuePageAssert _assert;
        private IssueDto _issue;
        private IssuePageView _page;
        private int _isueId;

        public WhenUpdatingIssuePageWhenPageDoesNotExist()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _issue = IssueBuilder.WithLibrary(LibraryId).Build();
            _page = new IssuePageView
            {
                IssueNumber = _issue.IssueNumber,
                Text = RandomData.Text,
                SequenceNumber = RandomData.Number
            };
            _isueId = _issue.Id;

            _response = await Client.PostObject($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/pages", _page);
            _assert = IssuePageAssert.FromResponse(_response, LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            BookBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResponse()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            _assert.ShouldHaveCorrectLocationHeader();
        }

        [Test]
        public void ShouldHaveReturnCorrectObject()
        {
            _assert.ShouldMatch(new IssuePageView
            {
                PeriodicalId = _issue.PeriodicalId,
                VolumeNumber = _issue.VolumeNumber,
                IssueNumber = _issue.IssueNumber,
                SequenceNumber = 1,
                Text = _page.Text,
                Status = "Available",
                WriterAccountId = _page.WriterAccountId,
                WriterAssignTimeStamp = _page.WriterAssignTimeStamp,
                ReviewerAccountId = _page.ReviewerAccountId,
                ReviewerAssignTimeStamp = _page.ReviewerAssignTimeStamp
            });
        }

        [Test]
        public void ShouldHaveSavedBookPage()
        {
            _assert.ShouldHaveSavedPage(DatabaseConnection);
        }
    }
}
