using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library
{
    [TestFixture(AuthenticationLevel.Unauthorized, true)]
    [TestFixture(AuthenticationLevel.Unauthorized, false)]
    [TestFixture(AuthenticationLevel.Reader, true)]
    [TestFixture(AuthenticationLevel.Reader, false)]
    public class WhenGettingLibraryWithReadOnlyPermissions : TestBase
    {
        private HttpResponseMessage _response;
        private LibraryView _view;

        public WhenGettingLibraryWithReadOnlyPermissions(AuthenticationLevel authLevel, bool periodicalsEnabled)
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
            if (_authenticationLevel == AuthenticationLevel.Unauthorized)
            {
                _view.Links.AssertLinkNotPresent("recents");
            }
            else
            {
                _view.Links.AssertLink("recents")
                    .ShouldBeGet()
                    .EndingWith($"/library/{LibraryId}/books")
                    .ShouldHaveQueryParameter("read", bool.TrueString);
            }
        }

        [Test]
        public void ShouldNotHaveWritableLinks()
        {
            _view.Links.AssertLinkNotPresent("create-book");
            _view.Links.AssertLinkNotPresent("create-category");
            _view.Links.AssertLinkNotPresent("create-series");
            _view.Links.AssertLinkNotPresent("create-author");
            _view.Links.AssertLinkNotPresent("create");
            _view.Links.AssertLinkNotPresent("update");
            _view.Links.AssertLinkNotPresent("delete");
        }
    }
}
