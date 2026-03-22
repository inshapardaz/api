using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.IssuePage.AssignPage
{
    [TestFixture]
    public class WhenAssigningIssuePageWhenPageIsComplete() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithPages(3).WithStatus(EditingStatus.Completed, 3).Build();
            var page = IssueBuilder.GetPages(issue.Id).PickRandom();
            var assignment = new
            {
                AccountId = AccountId
            };

            _response = await Client.PostObject($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/pages/{page.SequenceNumber}/assign", assignment); 
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            BookBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveBadRequestResponse() => _response.ShouldBeBadRequest();
    }
}
