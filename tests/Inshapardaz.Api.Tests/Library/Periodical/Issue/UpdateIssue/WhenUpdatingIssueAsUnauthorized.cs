using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.UpdateIssue
{
    [TestFixture]
    public class WhenUpdatingIssueAsUnauthorized
        : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issues = IssueBuilder.WithLibrary(LibraryId).WithContent().Build(4);
            var expected = issues.PickRandom();

            var updatedIssue = new IssueView {  IssueDate = RandomData.Date };
            _response = await Client.PutObject($"/libraries/{LibraryId}/periodicals/{expected.PeriodicalId}/volumes/{expected.VolumeNumber}/issues/{expected.IssueNumber}", updatedIssue);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorizedResult()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
