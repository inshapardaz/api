using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.UploadIssueImage
{
    [TestFixture]
    public class WhenUploadingIssueImageWhenNoExistingImage() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;
        private IssueAssert _assert;
        private int _issueId;
        private byte[] _newImage = RandomData.Bytes;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithNoImage().Build();
            _issueId = issue.Id;

            _response = await Client.PutFile($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/image", _newImage);
            _assert = Services.GetService<IssueAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            BookBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveHttpResponseMessage() => _response.ShouldBeCreated();

        [Test]
        public void ShouldHaveLocationHeader() => _assert.ShouldHaveCorrectImageLocationHeader(_issueId);

        [Test]
        public void ShouldHaveAddedImageToIssue() => _assert.ShouldHaveAddedIssueImage(_issueId);

        [Test]
        public void ShouldHavePublicImage() => _assert.ShouldHavePublicImage(_issueId);

        [Test]
        public void ShouldReturnChecksum() => _assert.ShouldHaveChecksum(_newImage);
    }
}
