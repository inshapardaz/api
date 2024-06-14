using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.issue.DeleteIssueContent
{
    [TestFixture]
    public class WhenDeletingIssueContentAsUnauthorised
        : TestBase
    {
        private HttpResponseMessage _response;

        private IssueContentDto _expected;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithContent().Build();
            _expected = IssueBuilder.Contents.PickRandom();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/contents?language={_expected.Language}", _expected.MimeType);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            _response.ShouldBeUnauthorized();
        }

        [Test]
        public void ShouldNotDeletedContent()
        {
            IssueContentAssert.ShouldHaveIssueContent(_expected.Id, _expected.Language, _expected.MimeType, DatabaseConnection);
        }
    }
}
