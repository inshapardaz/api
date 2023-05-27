using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookShelf.GetBookShelfById
{
    [TestFixture]
    public class WhenGettingSeriesById : TestBase
    {
        private HttpResponseMessage _response;
        private SeriesDto _expected;
        private SeriesAssert _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var series = SeriesBuilder.WithLibrary(LibraryId).WithBooks(3).WithoutImage().Build(4);
            _expected = series.PickRandom();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/series/{_expected.Id}");
            _assert = SeriesAssert.WithResponse(_response).InLibrary(LibraryId);
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
        public void ShouldNoHaveImageLink()
        {
            _assert.ShouldNotHaveImageLink();
        }

        [Test]
        public void ShouldReturnCorrectSeriesData()
        {
            _assert.ShouldHaveCorrectSeriesRetunred(_expected, DatabaseConnection);
        }
    }
}
