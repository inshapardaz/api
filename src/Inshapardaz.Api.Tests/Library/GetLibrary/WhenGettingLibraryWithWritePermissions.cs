using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.GetLibrary
{
    [TestFixture(Role.Admin, true)]
    [TestFixture(Role.Admin, false)]
    [TestFixture(Role.LibraryAdmin, true)]
    [TestFixture(Role.LibraryAdmin, false)]
    [TestFixture(Role.Writer, true)]
    [TestFixture(Role.Writer, false)]
    public class WhenGettingLibraryWithWritePermissions : TestBase
    {
        private HttpResponseMessage _response;
        private LibraryAssert _assert;

        public WhenGettingLibraryWithWritePermissions(Role authLevel, bool periodicalsEnabled)
            : base(authLevel, periodicalsEnabled)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.GetAsync($"/library/{LibraryId}");

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
            if (_role == Role.Admin)
            {
                _assert.ShouldHaveCreateCategorylink()
                    .ShouldHaveCreatelink()
                    .ShouldHaveUpdateLink()
                    .ShouldHaveDeleteLink();
            }
            else if (_role == Role.LibraryAdmin)
            {
                _assert.ShouldHaveCreateCategorylink()
                       .ShouldHaveUpdateLink();
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
