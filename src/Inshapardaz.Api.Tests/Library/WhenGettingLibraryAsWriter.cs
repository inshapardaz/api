using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library
{
    [TestFixture(AuthenticationLevel.Admin, true)]
    [TestFixture(AuthenticationLevel.Admin, false)]
    [TestFixture(AuthenticationLevel.LibraryAdmin, true)]
    [TestFixture(AuthenticationLevel.LibraryAdmin, false)]
    [TestFixture(AuthenticationLevel.Writer, true)]
    [TestFixture(AuthenticationLevel.Writer, false)]
    public class WhenGettingLibraryWithWritePermissions : TestBase
    {
        private HttpResponseMessage _response;
        private LibraryView _view;

        public WhenGettingLibraryWithWritePermissions(AuthenticationLevel authLevel, bool periodicalsEnabled)
            : base(periodicalsEnabled, authLevel)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var client = CreateClient();
            _response = await client.GetAsync($"/library/{LibraryId}");

            _view = await _response.GetContent<LibraryView>();
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
            _view.Links.AssertLink("self")
                       .ShouldBeGet()
                       .EndingWith($"/library/{LibraryId}");
        }

        [Test]
        public void ShouldHaveBooksLink()
        {
            _view.Links.AssertLink("books")
                       .ShouldBeGet()
                       .EndingWith($"/library/{LibraryId}/books");
        }

        [Test]
        public void ShouldHaveAuthorsLink()
        {
            _view.Links.AssertLink("authors")
                       .ShouldBeGet()
                       .EndingWith($"/library/{LibraryId}/authors");
        }

        [Test]
        public void ShouldHaveCategoriesLink()
        {
            _view.Links.AssertLink("categories")
                       .ShouldBeGet()
                       .EndingWith($"/library/{LibraryId}/categories");
        }

        [Test]
        public void ShouldHaveSeriesLink()
        {
            _view.Links.AssertLink("series")
                       .ShouldBeGet()
                       .EndingWith($"/library/{LibraryId}/series");
        }

        [Test]
        public void ShouldHaveCorrectPeriodicalLink()
        {
            if (_periodicalsEnabled)
            {
                _view.Links.AssertLink("periodicals")
                       .ShouldBeGet()
                       .EndingWith($"/library/{LibraryId}/periodicals");
            }
            else
            {
                _view.Links.AssertLinkNotPresent("periodicals");
            }
        }

        [Test]
        public void ShouldNotHavePersonalLinks()
        {
            _view.Links.AssertLink("recents")
                .ShouldBeGet()
                .EndingWith($"/library/{LibraryId}/books")
                .ShouldHaveQueryParameter("read", bool.TrueString);
        }

        [Test]
        public void ShouldHaveWritableLinks()
        {
            if (_authenticationLevel == AuthenticationLevel.Admin ||
                _authenticationLevel == AuthenticationLevel.LibraryAdmin)
            {
                _view.Links.AssertLink("create-category")
                       .ShouldBePost()
                       .EndingWith($"/library/{LibraryId}/categories");
            }
            else
            {
                _view.Links.AssertLinkNotPresent("create-category");
            }
        }
    }
}
