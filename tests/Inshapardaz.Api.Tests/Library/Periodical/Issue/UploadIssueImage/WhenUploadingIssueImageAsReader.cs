using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.UploadIssueImage
{
    [TestFixture]
    public class WhenUploadingIssueImageAsReader : TestBase
    {
        private HttpResponseMessage _response;
        private int _issueId;
        private byte[] _oldImage;

        public WhenUploadingIssueImageAsReader()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).Build();
            _issueId = issue.Id;
            var imageUrl = DatabaseConnection.GetIssueImageUrl(_issueId);
            _oldImage = await FileStore.GetFile(imageUrl, CancellationToken.None);

            _response = await Client.PutFile($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/image", RandomData.Bytes);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            BookBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbidResult()
        {
            _response.ShouldBeForbidden();
        }

        [Test]
        public void ShouldNotHaveUpdatedIssueImage()
        {
            IssueAssert.ShouldNotHaveUpdatedIssueImage(DatabaseConnection, FileStore, _issueId, _oldImage);
        }
    }
}
