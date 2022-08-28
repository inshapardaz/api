using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.IssuePage.UpdatePage
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenUpdatingIssuePageWithPermissions : TestBase
    {
        private HttpResponseMessage _response;
        private IssuePageAssert _assert;
        private IssueDto _issue;
        private IssuePageDto _page;
        private IssuePageView _updatedPage;
        private int _issueId;

        public WhenUpdatingIssuePageWithPermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _issue = IssueBuilder.WithLibrary(LibraryId).WithPages(3, true).Build();
            _page = IssueBuilder.GetPages(_issue.Id).PickRandom();

            _updatedPage = new IssuePageView
            {
                IssueNumber = _issue.IssueNumber,
                Text = RandomData.Text,
                SequenceNumber = _page.SequenceNumber
            };

            _issueId = _issue.Id;
            _response = await Client.PutObject($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/pages/{_page.SequenceNumber}", _updatedPage);
            _assert = IssuePageAssert.FromResponse(_response, LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            BookBuilder.CleanUp();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveReturnCorrectObject()
        {
            _assert.ShouldMatch(new IssuePageView
            {
                PeriodicalId = _issue.PeriodicalId,
                VolumeNumber = _issue.VolumeNumber,
                IssueNumber = _issue.IssueNumber,
                SequenceNumber = _page.SequenceNumber,
                Text = _updatedPage.Text,
                Status = "Available",
                ReviewerAccountId = _page.ReviewerAccountId,
                ReviewerAssignTimeStamp = _page.ReviewerAssignTimeStamp,
                WriterAccountId = _page.WriterAccountId,
                WriterAssignTimeStamp = _page.WriterAssignTimeStamp
            });
        }

        [Test]
        public void ShouldHaveSavedBookPage()
        {
            _assert.ShouldHaveSavedPage(DatabaseConnection);
        }
    }
}
