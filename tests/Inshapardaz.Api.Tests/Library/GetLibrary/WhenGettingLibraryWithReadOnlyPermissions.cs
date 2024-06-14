using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.GetLibrary
{
    [TestFixture(Role.Reader, true)]
    [TestFixture(Role.Reader, false)]
    public class WhenGettingLibraryWithReadOnlyPermissions : TestBase
    {
        private HttpResponseMessage _response;
        private LibraryAssert _assert;

        public WhenGettingLibraryWithReadOnlyPermissions(Role authLevel, bool periodicalsEnabled)
            : base(authLevel, periodicalsEnabled)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.GetAsync($"/libraries/{LibraryId}");

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
        public void ShouldNotHavePersonalLinks()
        {
            if (_role == null)
            {
                _assert.ShouldNotHaveRecentLinks();
            }
            else
            {
                _assert.ShouldHaveRecentLinks();
            }
        }

        [Test]
        public void ShouldNotHaveWritableLinks()
        {
            _assert.ShouldNotHaveCreateCategoryLink()
                    .ShouldNotHaveUpdatelink()
                    .ShouldNotHaveDeletelink()
                    .ShouldNotHaveCreateBookLink()
                    .ShouldNotHaveCreateSeriesLink()
                    .ShouldNotHaveCreateAuthorLink();
        }
    }
}
