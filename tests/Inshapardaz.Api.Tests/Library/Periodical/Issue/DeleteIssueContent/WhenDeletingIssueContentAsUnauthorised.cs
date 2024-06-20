using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Microsoft.Extensions.DependencyInjection;
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
        private IssueContentAssert _assert;
        private IssueContentDto _expected;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithContent().Build();
            _expected = IssueBuilder.Contents.PickRandom();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/contents/{_expected.Id}?language={_expected.Language}", _expected.MimeType);
            _assert = Services.GetService<IssueContentAssert>().ForResponse(_response).ForLibrary(Library);
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
            _assert.ShouldHaveIssueContent(_expected.Id, _expected.Language, _expected.MimeType);
        }
    }
}
