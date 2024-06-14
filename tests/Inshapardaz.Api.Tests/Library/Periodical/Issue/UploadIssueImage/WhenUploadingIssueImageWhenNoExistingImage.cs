using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.UploadIssueImage
{
    [TestFixture]
    public class WhenUploadingIssueImageWhenNoExistingImage : TestBase
    {
        private HttpResponseMessage _response;
        private IssueAssert _issueAssert;
        private int _issueId;

        public WhenUploadingIssueImageWhenNoExistingImage()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithNoImage().Build();
            _issueId = issue.Id;

            _response = await Client.PutFile($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/image", RandomData.Bytes);
            _issueAssert = IssueAssert.FromResponse(_response).InLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            BookBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveHttpResponseMessage()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            _issueAssert.ShouldHaveCorrectImageLocationHeader(_issueId);
        }

        [Test]
        public void ShouldHaveAddedImageToIssue()
        {
            IssueAssert.ShouldHaveAddedIssueImage(DatabaseConnection, FileStore, _issueId);
        }

        [Test]
        public void ShouldHavePublicImage()
        {
            IssueAssert.ShouldHavePublicImage(DatabaseConnection, _issueId);
        }
    }
}
