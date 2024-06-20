using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.IssuePage.AssignPage
{
    [TestFixture(Role.Admin, EditingStatus.Available)]
    [TestFixture(Role.LibraryAdmin, EditingStatus.Available)]
    [TestFixture(Role.Writer, EditingStatus.Available)]
    [TestFixture(Role.Writer, EditingStatus.Typing)]
    [TestFixture(Role.Writer, EditingStatus.Typed)]
    [TestFixture(Role.Writer, EditingStatus.InReview)]
    public class WhenAssigningIssuePageToSomeoneElse : TestBase
    {
        private HttpResponseMessage _response;
        private IssuePageAssert _assert;
        private IssueDto _issue;
        private IssuePageDto _page;
        private AccountDto _secondAccount;
        private readonly EditingStatus _status;

        public WhenAssigningIssuePageToSomeoneElse(Role role, EditingStatus status)
            : base(role)
        {
            _status = status;
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _secondAccount = AccountBuilder.Build();
            _issue = IssueBuilder.WithLibrary(LibraryId).WithPages(3, true).WithStatus(_status, 3).Build();
            _page = IssueBuilder.GetPages(_issue.Id).PickRandom();

            var assignment = new
            {
                AccountId = _secondAccount.Id
            };

            _response = await Client.PostObject($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/pages/{_page.SequenceNumber}/assign", assignment);
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
        public void ShouldHaveCorrectPageAssignment()
        {
            bool ShouldAssignReview()
            {
                return _status == EditingStatus.Typed || _status == EditingStatus.InReview;
            }

            _assert.ShouldMatch(new IssuePageView
            {
                PeriodicalId = _issue.PeriodicalId,
                VolumeNumber = _issue.VolumeNumber,
                IssueNumber = _issue.IssueNumber,
                SequenceNumber = _page.SequenceNumber,
                Text = _page.Text,
                Status = _status.ToString(),
                ReviewerAccountId = ShouldAssignReview() ? _secondAccount.Id : _page.ReviewerAccountId,
                ReviewerAccountName = ShouldAssignReview() ? _secondAccount.Name : null,
                ReviewerAssignTimeStamp = ShouldAssignReview() ? System.DateTime.UtcNow : _page.ReviewerAssignTimeStamp,
                WriterAccountId = !ShouldAssignReview() ? _secondAccount.Id : _page.ReviewerAccountId,
                WriterAccountName = !ShouldAssignReview() ? _secondAccount.Name : null,
                WriterAssignTimeStamp = !ShouldAssignReview() ?  System.DateTime.UtcNow : _page.WriterAssignTimeStamp
            });            
        }

        [Test]
        public void ShouldHaveCorrectAssignmentTimeStamp()
        {
            _assert.ShouldHaveAssignedRecently();
        }
    }
}
