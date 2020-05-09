using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library
{
    [TestFixture(AuthenticationLevel.Administrator, true)]
    [TestFixture(AuthenticationLevel.Administrator, false)]
    [TestFixture(AuthenticationLevel.Writer, true)]
    [TestFixture(AuthenticationLevel.Writer, false)]
    public class WhenGettingLibraryAsWriter
        : LibraryTest<GetLibrary>
    {
        private OkObjectResult _response;
        private LibraryView _view;
        private AuthenticationLevel _authenticationLevel;

        public WhenGettingLibraryAsWriter(AuthenticationLevel authLevel, bool periodicalsEnabled)
            : base(periodicalsEnabled)
        {
            _authenticationLevel = authLevel;
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            var claims = AuthenticationBuilder.CreateClaim(_authenticationLevel);

            _response = (OkObjectResult)await handler.Run(request, LibraryId, claims);

            _view = _response.Value as LibraryView;
        }

        [Test]
        public void ShouldReturnOkResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void ShouldReturnOk()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo(200));
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
            if (PeriodicalsEnabled)
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
                .EndingWith($"/library/{LibraryId}/recents");
            _view.Links.AssertLink("favorites")
                .ShouldBeGet()
                .EndingWith($"/library/{LibraryId}/favorites");
        }

        [Test]
        public void ShouldHaveWritableLinks()
        {
            if (_authenticationLevel == AuthenticationLevel.Administrator)
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
