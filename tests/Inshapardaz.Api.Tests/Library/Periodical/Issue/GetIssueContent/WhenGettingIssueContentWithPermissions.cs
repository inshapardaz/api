using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.GetIssueContent
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenGettingIssueContentWithPermissions
        : TestBase
    {
        private HttpResponseMessage _response;
        private IssueContentAssert _assert;
        private IssueDto _issue;
        private IssueContentDto _expected;

        public WhenGettingIssueContentWithPermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _issue = IssueBuilder.WithLibrary(LibraryId).WithContents(5).Build();
            _expected = IssueBuilder.Contents.PickRandom();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/contents/{_expected.Id}?language={_expected.Language}", _expected.MimeType);
            _assert = Services.GetService<IssueContentAssert>().ForResponse(_response).ForLibrary(Library);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _assert.ShouldHaveSelfLink();
        }

        [Test]
        public void ShouldHaveEditLinks()
        {
            _assert.ShouldHaveUpdateLink();
            _assert.ShouldHaveDeleteLink();
        }

        [Test]
        public void ShouldHaveCorrectMimeType()
        {
            _assert.ShouldHaveCorrectMimeType(_expected.MimeType);
        }

        [Test]
        public void ShouldHaveCorrectLanguage()
        {
            _assert.ShouldHaveCorrectLanguage(_expected.Language);
        }

        [Test]
        public void SHouldHaveDownloadLink()
        {
            _assert.ShouldHavePrivateDownloadLink();
        }

        [Test]
        public void ShouldReturnCorrectChapterData()
        {
            _assert.ShouldMatch(_expected, _expected.Id);
        }
    }
}
