using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.IssuePage.UploadPageImage
{
    [TestFixture]
    public class WhenUploadingIssuePageImageWhenNoExistingImage : TestBase
    {
        private HttpResponseMessage _response;
        private IssuePageAssert _assert;
        private IssuePageDto _page;
        private int _issueId;

        public WhenUploadingIssuePageImageWhenNoExistingImage()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithPages(3).Build();
            _page = IssueBuilder.GetPages(issue.Id).PickRandom();
            _issueId = issue.Id;
            _response = await Client.PutFile($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/pages/{_page.SequenceNumber}/image", RandomData.Bytes);
            _assert = Services.GetService<IssuePageAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            BookBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResponse()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            var savedPage = IssuePageTestRepository.GetIssuePageByNumber(_page.IssueId, _page.SequenceNumber);
            _assert.ShouldHaveCorrectImageLocationHeader(_response, savedPage.ImageId.Value);
        }

        [Test]
        public void ShouldHaveAddedImageToIssue()
        {
            _assert.ShouldHaveAddedIssuePageImage(_issueId, _page.SequenceNumber);
        }
    }
}
