using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.GetLibrary
{
    [TestFixture(Permission.Admin, true)]
    [TestFixture(Permission.Admin, false)]
    [TestFixture(Permission.LibraryAdmin, true)]
    [TestFixture(Permission.LibraryAdmin, false)]
    [TestFixture(Permission.Writer, true)]
    [TestFixture(Permission.Writer, false)]
    public class WhenGettingLibraryWithWritePermissions : TestBase
    {
        private HttpResponseMessage _response;
        private LibraryAssert _assert;

        public WhenGettingLibraryWithWritePermissions(Permission authLevel, bool periodicalsEnabled)
            : base(authLevel, periodicalsEnabled)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var client = CreateClient();
            _response = await client.GetAsync($"/library/{LibraryId}");

            _assert = LibraryAssert.FromResponse(_response, LibraryId);
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
        public void ShouldHaveAuthorsLink()
        {
            _assert.ShouldHaveAuthorsLink();
        }

        [Test]
        public void ShouldHaveCategoriesLink()
        {
            _assert.ShouldHaveCategoriesLink();
        }

        [Test]
        public void ShouldHaveSeriesLink()
        {
            _assert.ShouldHaveSeriesLink();
        }

        [Test]
        public void ShouldHaveCorrectPeriodicalLink()
        {
            if (_periodicalsEnabled)
            {
                _assert.ShouldHavePeriodicalLink();
            }
            else
            {
                _assert.ShouldNotHavePeriodicalLink();
            }
        }

        [Test]
        public void ShouldHaveRecentLinks()
        {
            _assert.ShouldHaveRecentLinks();
        }

        [Test]
        public void ShouldHaveWritableLinks()
        {
            if (_authenticationLevel == Permission.Admin)
            {
                _assert.ShouldHaveCreateCategorylink()
                    .ShouldHaveCreatelink()
                    .ShouldHaveUpdatLlink()
                    .ShouldHaveDeleteLink();
            }
            else if (_authenticationLevel == Permission.LibraryAdmin)
            {
                _assert.ShouldHaveCreateCategorylink()
                       .ShouldHaveUpdatLlink();
            }
            else
            {
                _assert.ShouldNotHaveCreateCategorylink()
                    .ShouldNotHaveCreatelink()
                    .ShouldNotHaveUpdatelink()
                    .ShouldNotHaveDeletelink();
            }
        }
    }
}
