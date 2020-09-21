using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.GetLibrary
{
    [TestFixture(Permission.Unauthorised, true)]
    [TestFixture(Permission.Unauthorised, false)]
    [TestFixture(Permission.Reader, true)]
    [TestFixture(Permission.Reader, false)]
    public class WhenGettingLibraryWithReadOnlyPermissions : TestBase
    {
        private HttpResponseMessage _response;
        private LibraryAssert _assert;

        public WhenGettingLibraryWithReadOnlyPermissions(Permission authLevel, bool periodicalsEnabled)
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
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
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
            if (_authenticationLevel == Permission.Unauthorised)
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
            _assert.ShouldNotHaveCreateCategorylink()
                    .ShouldNotHaveCreatelink()
                    .ShouldNotHaveUpdatelink()
                    .ShouldNotHaveDeletelink()
                    .ShouldNotHaveCreateBookLink()
                    .ShouldNotHaveSerieslink()
                    .ShouldNotHaveCreateAuthorLink();
        }
    }
}
