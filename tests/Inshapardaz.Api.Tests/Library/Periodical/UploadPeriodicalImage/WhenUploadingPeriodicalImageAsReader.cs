using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.UploadPeriodicalImage
{
    [TestFixture]
    public class WhenUploadingPeriodicalImageAsReader : TestBase
    {
        private HttpResponseMessage _response;
        private int _periodicalId;
        private byte[] _oldImage;

        public WhenUploadingPeriodicalImageAsReader()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var periodical = PeriodicalBuilder.WithLibrary(LibraryId).Build();
            _periodicalId = periodical.Id;
            var imageUrl = DatabaseConnection.GetPeriodicalImageUrl(_periodicalId);
            _oldImage = await FileStore.GetFile(imageUrl, CancellationToken.None);

            _response = await Client.PutFile($"/libraries/{LibraryId}/periodicals/{_periodicalId}/image", RandomData.Bytes);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            PeriodicalBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbidResult()
        {
            _response.ShouldBeForbidden();
        }

        [Test]
        public void ShouldNotHaveUpdatedPeriodicalImage()
        {
            PeriodicalAssert.ShouldNotHaveUpdatedPeriodicalImage(_periodicalId, _oldImage, DatabaseConnection, FileStore);
        }
    }
}
