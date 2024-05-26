using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.UpdateIssueContent
{
    [TestFixture]
    public class WhenUpdatingIssueContentForNonExistingIssue
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenUpdatingIssueContentForNonExistingIssue() 
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var newContents = RandomData.Bytes;

            _response = await Client.PutFile($"/libraries/{LibraryId}/periodicals/{-RandomData.Number}/volumes/{-RandomData.Number}/issues/{-RandomData.Number}/contents", newContents, RandomData.Locale, RandomData.MimeType);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnBadRequestResult()
        {
            _response.ShouldBeBadRequest();
        }
    }
}
