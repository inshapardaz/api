using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.GetIssueContent
{
    [TestFixture]
    public class WhenGettingIssueContentWithoutLanguage
        : TestBase
    {
        private HttpResponseMessage _response;
        private IssueContentAssert _assert;
        private IssueDto _issue;
        private IssueContentDto _expected;

        public WhenGettingIssueContentWithoutLanguage()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _issue = IssueBuilder.WithLibrary(LibraryId).WithContents(1).WithContentLanguage(Library.Language).Build();
            _expected = IssueBuilder.Contents.Single(x => x.Language == Library.Language);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/contents/{_expected.Id}", _expected.MimeType);
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
            _assert.ShouldHaveCorrectLanguage(Library.Language);
        }

        [Test]
        public void ShouldNotHaveEditLinks()
        {
            _assert.ShouldNotHaveUpdateLink();
            _assert.ShouldNotHaveDeleteLink();
        }

        [Test]
        public void ShouldHaveCorrectMimeType()
        {
            _assert.ShouldHaveCorrectMimeType(_expected.MimeType);
        }

        [Test]
        public void ShouldHaveCorrectLanguage()
        {
            _assert.ShouldHaveCorrectLanguage(Library.Language);
        }

        [Test]
        public void SHouldHaveDownloadLink()
        {
            _assert.ShouldHavePrivateDownloadLink();
        }

        [Test]
        public void ShouldReturnCorrectContentData()
        {
            _assert.ShouldMatch(_expected, _expected.Id);
        }
    }
}
