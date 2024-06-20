using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.UpdateIssueContent
{
    [TestFixture]
    public class WhenUpdatingIssueContentAsUnauthorisedUser
        : TestBase
    {
        private HttpResponseMessage _response;

        private IssueDto _issue;
        private byte[] _expected;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _issue = IssueBuilder.WithLibrary(LibraryId).WithContent().Build();
            var content = IssueBuilder.Contents.PickRandom();

            _expected = RandomData.Bytes;
            _response = await Client.PutFile($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/contents/{content.Id}?language={content.Language}", _expected, content.MimeType);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnUnauthorizedResult()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
