using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Extensions;

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
        private AccountDto _newWriter;
        private AccountDto _newReviewer;

        public WhenUpdatingIssuePageWithPermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _issue = IssueBuilder.WithLibrary(LibraryId).WithPages(3, true).Build();
            _page = IssueBuilder.GetPages(_issue.Id).PickRandom();
            _newWriter = AccountBuilder.Build();
            _newReviewer = AccountBuilder.Build();

            _updatedPage = new IssuePageView
            {
                Text = RandomData.Text,
                Status = RandomData.EditingStatus.ToDescription(),
                ReviewerAccountId = _newReviewer.Id,
                ReviewerAssignTimeStamp = RandomData.Date,
                WriterAccountId = _newWriter.Id,
                WriterAssignTimeStamp = RandomData.Date,
            };

            _issueId = _issue.Id;
            _response = await Client.PutObject($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/pages/{_page.SequenceNumber}", _updatedPage);
            _assert = Services.GetService<IssuePageAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
                Status = _updatedPage.Status,
                ReviewerAccountId = _updatedPage.ReviewerAccountId,
                ReviewerAccountName = _newReviewer.Name,
                ReviewerAssignTimeStamp = _updatedPage.ReviewerAssignTimeStamp,
                WriterAccountId = _updatedPage.WriterAccountId,
                WriterAccountName = _newWriter.Name,
                WriterAssignTimeStamp = _updatedPage.WriterAssignTimeStamp
            });
        }

        [Test]
        public void ShouldHaveSavedBookPage()
        {
            _assert.ShouldHaveSavedPage();
        }
    }
}
