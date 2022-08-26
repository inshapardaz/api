using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.issue.DeleteIssueContent
{
    [TestFixture]
    public class WhenDeletingIssueContentAsReader
        : TestBase
    {
        private HttpResponseMessage _response;
        private IssueContentDto _expected;

        public WhenDeletingIssueContentAsReader()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithContents(2).Build();
            _expected = IssueBuilder.Contents.PickRandom();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/contents", _expected.Language, _expected.MimeType);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnForbidden()
        {
            _response.ShouldBeForbidden();
        }

        [Test]
        public void ShouldNotDeletedContent()
        {
            IssueContentAssert.ShouldHaveIssueContent(_expected.Id, _expected.Language, _expected.MimeType, DatabaseConnection);
        }
    }
}
