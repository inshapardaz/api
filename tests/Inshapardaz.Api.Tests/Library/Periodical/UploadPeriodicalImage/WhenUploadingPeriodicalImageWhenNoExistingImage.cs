using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.UploadPeriodicalImage
{
    [TestFixture]
    public class WhenUploadingPeriodicalImageWhenNoExistingImage : TestBase
    {
        private HttpResponseMessage _response;
        private PeriodicalAssert _assert;
        private int _periodicalId;

        public WhenUploadingPeriodicalImageWhenNoExistingImage()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var periodical = PeriodicalBuilder.WithLibrary(LibraryId).WithNoImage().Build();
            _periodicalId = periodical.Id;

            _response = await Client.PutFile($"/libraries/{LibraryId}/periodicals/{_periodicalId}/image", RandomData.Bytes);
            _assert = Services.GetService<PeriodicalAssert>().ForResponse(_response)
                    .ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            PeriodicalBuilder.CleanUp();
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
            _assert.ShouldHaveImageLocationHeader();
        }

        [Test]
        public void ShouldHaveAddedImageToPeriodical()
        {
            _assert.ShouldHaveAddedPeriodicalImage(_periodicalId);
        }

        [Test]
        public void ShouldHaveImage()
        {
            _assert.ShouldHavePublicImage(_periodicalId);
        }
    }
}
