using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.UploadPeriodicalImage
{
    [TestFixture]
    public class WhenUploadingPeriodicalImageAsUnauthorized : TestBase
    {
        private HttpResponseMessage _response;
        private int _periodicalId;
        private byte[] _oldImage;

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
        public void Teardown()
        {
            PeriodicalBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorisedResult()
        {
            _response.ShouldBeUnauthorized();
        }

        [Test]
        public void ShouldNotHaveUpdatedPeriodicalImage()
        {
            PeriodicalAssert.ShouldNotHaveUpdatedPeriodicalImage(_periodicalId, _oldImage, DatabaseConnection, FileStore);
        }
    }
}
