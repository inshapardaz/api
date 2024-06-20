using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.UploadPeriodicalImage
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenUploadingPeriodicalImageWithPermissions : TestBase
    {
        private HttpResponseMessage _response;
        private PeriodicalAssert _assert;
        private int _periodicalId;
        private byte[] _newImage = RandomData.Bytes;

        public WhenUploadingPeriodicalImageWithPermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var priodical = PeriodicalBuilder.WithLibrary(LibraryId).Build();
            _periodicalId = priodical.Id;

            var imageUrl = PeriodicalTestRepository.GetPeriodicalImageUrl(_periodicalId);

            _response = await Client.PutFile($"/libraries/{LibraryId}/periodicals/{_periodicalId}/image", _newImage);

            _assert = Services.GetService<PeriodicalAssert>().ForResponse(_response)
                   .ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            PeriodicalBuilder.CleanUp();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveUpdatedPeriodicalImage()
        {
            _assert.ShouldHaveUpdatedPeriodicalImage(_periodicalId, _newImage);
        }
    }
}
