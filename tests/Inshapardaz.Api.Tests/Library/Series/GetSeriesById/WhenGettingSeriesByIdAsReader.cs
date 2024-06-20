using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Series.GetSeriesById
{
    [TestFixture]
    public class WhenGettingSeriesByIdAsReader : TestBase
    {
        private HttpResponseMessage _response;
        private SeriesDto _expected;
        private SeriesAssert _assert;

        public WhenGettingSeriesByIdAsReader()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var series = SeriesBuilder.WithLibrary(LibraryId).Build(4);
            _expected = series.PickRandom();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/series/{_expected.Id}");
            _assert = Services.GetService<SeriesAssert>().ForResponse(_response).InLibrary(LibraryId);
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
        }

        [Test]
        public void ShouldHaveBooksLink()
        {
            _assert.ShouldHaveBooksLink();
        }

        [Test]
        public void ShouldNotHaveUpdateLink()
        {
            _assert.ShouldNotHaveUpdateLink();
        }

        [Test]
        public void ShouldNotHaveDeleteLink()
        {
            _assert.ShouldNotHaveDeleteLink();
        }

        [Test]
        public void ShouldNotHaveImageUploadLink()
        {
            _assert.ShouldNotHaveImageUploadLink();
        }

        [Test]
        public void ShouldReturnCorrectSeriesData()
        {
            _assert.ShouldHaveCorrectSeriesRetunred(_expected);
        }
    }
}
