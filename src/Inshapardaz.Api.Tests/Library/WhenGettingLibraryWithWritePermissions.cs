using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library
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
        private LibraryView _view;

        public WhenGettingLibraryWithWritePermissions(Permission authLevel, bool periodicalsEnabled)
            : base(authLevel, periodicalsEnabled)
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
            if (_authenticationLevel == Permission.Admin)
            {
                _view.Links.AssertLink("create-category")
                       .ShouldBePost()
                       .EndingWith($"/library/{LibraryId}/categories");
                _view.Links.AssertLink("create")
                        .ShouldBePost()
                        .EndingWith($"/library");
                _view.Links.AssertLink("update")
                        .ShouldBePut()
                        .EndingWith($"/library/{LibraryId}");
                _view.Links.AssertLink("delete")
                        .ShouldBeDelete()
                        .EndingWith($"/library/{LibraryId}");
            }
            else if (_authenticationLevel == Permission.LibraryAdmin)
            {
                _view.Links.AssertLink("create-category")
                       .ShouldBePost()
                       .EndingWith($"/library/{LibraryId}/categories");
                _view.Links.AssertLink("update")
                        .ShouldBePut()
                        .EndingWith($"/library/{LibraryId}");
            }
            else
            {
                _view.Links.AssertLinkNotPresent("create-category");
                _view.Links.AssertLinkNotPresent("create");
                _view.Links.AssertLinkNotPresent("update");
                _view.Links.AssertLinkNotPresent("delete");
            }
        }
    }
}
