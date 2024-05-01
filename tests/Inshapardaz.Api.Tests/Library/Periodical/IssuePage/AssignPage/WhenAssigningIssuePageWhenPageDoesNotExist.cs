using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.IssuePage.AssignPage
{
    [TestFixture]
    public class WhenAssigningIssuePageWhenPageDoesNotExist : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAssigningIssuePageWhenPageDoesNotExist()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithPages(3).Build();
            var page = IssueBuilder.GetPages(issue.Id).PickRandom();
            var assignment = new
            {
                Status = EditingStatus.InReview,
                AccountId = AccountId
            };

            _response = await Client.PostObject($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/pages/{-RandomData.Number}/assign", assignment); 
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            BookBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveBadRequestResponse()
        {
            _response.ShouldBeBadRequest();
        }
    }
}
