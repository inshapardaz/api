using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.UpdateIssueContent
{
    [TestFixture]
    public class WhenUpdatingIssueContentWithDifferentContentType
        : TestBase
    {
        private HttpResponseMessage _response;
        private string _newMimeType;
        private IssueDto _issue;
        private IssueContentDto _content;
        private byte[] _contents;
        private IssueContentAssert _assert;

        public WhenUpdatingIssueContentWithDifferentContentType()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _newMimeType = "text/markdown";

            _issue = IssueBuilder.WithLibrary(LibraryId).WithContents(2, "application/pdf").Build();
            _content = IssueBuilder.Contents.PickRandom();

            _contents = RandomData.Bytes;
            _response = await Client.PutFile($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/contents/{_content.Id}?language={_content.Language}", _contents, _newMimeType);
            _assert = Services.GetService<IssueContentAssert>().ForResponse(_response).ForLibrary(Library);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveCorrectLink()
        {
            _assert.ShouldHaveSelfLink()
                   .ShouldHavePeriodicalLink()
                   .ShouldHaveIssueLink()
                   .ShouldHaveUpdateLink()
                   .ShouldHaveDeleteLink();
        }

        [Test]
        public void ShouldHaveCorrectLanguage()
        {
            _assert.ShouldHaveCorrectLanguage(_content.Language);
        }

        [Test]
        public void ShouldHaveCorrectMimeType()
        {
            _assert.ShouldHaveCorrectMimeType(_newMimeType);
        }

        [Test]
        public void ShouldHaveCorrectContentSaved()
        {
            _assert.ShouldHaveCorrectContents(_contents, newMimeType: _newMimeType);
        }
    }
}
