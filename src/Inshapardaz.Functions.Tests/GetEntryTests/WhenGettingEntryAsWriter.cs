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
    public class WhenGettingEntryAsWriter : FunctionTest
    {
        OkObjectResult _response;
        EntryView _view;
        
        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            AuthenticateAsWriter();
            var handler = Container.GetService<GetEntry>();
            _response = (OkObjectResult) await handler.Run(request, NullLogger.Instance);

            _view = _response.Value as EntryView;
        }

        [Test]
        public void ShouldReturnOkResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void ShouldReturnSelfLink()
        {
            _view.Links.AssertLink("self")
                       .ShouldBeGet()
                       .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldReturnBooksLink()
        {
            _view.Links.AssertLink("books")
                       .ShouldBeGet()
                       .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldReturnLatestBooksLink()
        {
            _view.Links.AssertLink("latest")
                       .ShouldBeGet()
                       .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldReturnAuthorsLink()
        {
            _view.Links.AssertLink("authors")
                       .ShouldBeGet()
                       .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldReturnCategoriesLink()
        {
            _view.Links.AssertLink("categories")
                       .ShouldBeGet()
                       .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldReturnSeriesLink()
        {
            _view.Links.AssertLink("series")
                       .ShouldBeGet()
                       .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldReturnPeriodicalLink()
        {
            _view.Links.AssertLink("periodicals")
                       .ShouldBeGet()
                       .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldReturnDictionariesLink()
        {
            _view.Links.AssertLink("dictionaries")
                       .ShouldBeGet()
                       .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldReturnLanguagesLink()
        {
            _view.Links.AssertLink("languages")
                       .ShouldBeGet()
                       .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldReturnRelationshipTypesLink()
        {
            _view.Links.AssertLink("relationshiptypes")
                       .ShouldBeGet()
                       .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldReturnAttributesLink()
        {
            _view.Links.AssertLink("attributes")
                       .ShouldBeGet()
                       .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldHavePersonalLinks()
        {
            _view.Links.AssertLink("recents")
                       .ShouldBeGet()
                       .ShouldHaveSomeHref();
            _view.Links.AssertLink("favorites")
                       .ShouldBeGet()
                       .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldHaveWritableLinks()
        {
            _view.Links.AssertLink("image-upload")
                       .ShouldBePost()
                       .ShouldHaveSomeHref();
        }
    }
}