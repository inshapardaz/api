using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.issue.DeleteIssueContent
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingIssueContentWithPermissions
        : TestBase
    {
        private HttpResponseMessage _response;
        private IssueContentDto _expected;

        public WhenDeletingIssueContentWithPermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithContents(3).Build();
            _expected = IssueBuilder.Contents.PickRandom();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/contents?language={_expected.Language}", _expected.MimeType);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNoContentResult()
        {
            _response.ShouldBeNoContent();
        }

        [Test]
        public void ShouldHaveDeletedContent()
        {
            IssueContentAssert.ShouldNotHaveIssueContent(_expected.Id, DatabaseConnection);
        }

        [Test]
        public void ShouldNotHaveDeletedOtherContents()
        {
            foreach (var item in IssueBuilder.Contents)
            {
                if (item.Id != _expected.Id)
                {
                    IssueContentAssert.ShouldHaveIssueContent(item.Id, item.Language, item.MimeType, DatabaseConnection);
                }
            }
        }
    }
}
