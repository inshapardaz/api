using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.GetEntryTests
{
    [TestFixture]
    public class WhenGettingEntry : FunctionTest
    {
        OkObjectResult _response;
        EntryView _view;
        
        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            var handler = Container.GetService<GetEntry>();
            _response = (OkObjectResult) await handler.Run(request, NullLogger.Instance);

            _view = _response.Value as EntryView;
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _view.Links.AssertLink("self")
                       .ShouldBeGet()
                       .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldHaveBooksLink()
        {
            _view.Links.AssertLink("books")
                       .ShouldBeGet()
                       .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldHaveLatestBooksLink()
        {
            _view.Links.AssertLink("latest")
                       .ShouldBeGet()
                       .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldHaveAuthorsLink()
        {
            _view.Links.AssertLink("authors")
                       .ShouldBeGet()
                       .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldHaveCategoriesLink()
        {
            _view.Links.AssertLink("categories")
                       .ShouldBeGet()
                       .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldHaveSeriesLink()
        {
            _view.Links.AssertLink("series")
                       .ShouldBeGet()
                       .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldHavePeriodicalLink()
        {
            _view.Links.AssertLink("periodicals")
                       .ShouldBeGet()
                       .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldHaveDictionariesLink()
        {
            _view.Links.AssertLink("dictionaries")
                       .ShouldBeGet()
                       .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldHaveLanguagesLink()
        {
            _view.Links.AssertLink("languages")
                       .ShouldBeGet()
                       .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldHaveRelationshipTypesLink()
        {
            _view.Links.AssertLink("relationshiptypes")
                       .ShouldBeGet()
                       .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldHaveAttributesLink()
        {
            _view.Links.AssertLink("attributes")
                       .ShouldBeGet()
                       .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldNotHavePersonalLinks()
        {
            _view.Links.AssertLinkNotPresent("recent");
            _view.Links.AssertLinkNotPresent("favorites");
        }

        [Test]
        public void ShouldNotHaveWritableLinks()
        {
            _view.Links.AssertLinkNotPresent("image-upload");
        }
    }
}