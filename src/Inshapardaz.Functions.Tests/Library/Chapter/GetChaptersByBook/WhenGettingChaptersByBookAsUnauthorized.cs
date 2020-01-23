using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.GetChaptersByBook
{
    [TestFixture]
    public class WhenGettingChaptersByBookAsUnauthorized : FunctionTest
    {
        private OkObjectResult _response;
        private ListView<ChapterView> _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            var dataBuilder = Container.GetService<ChapterDataBuilder>();
            var chapters = dataBuilder.AsPublic().Build(4);
            var book = chapters.First().Book;

            var handler = Container.GetService<Functions.Library.Books.Chapters.GetChaptersByBook>();
            _response = (OkObjectResult) await handler.Run(null, book.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);

            _view = _response.Value as ListView<ChapterView>;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
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
                 .ShouldHaveSomeHref();
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _view.Links.AssertLinkNotPresent("create");
        }

         [Test]
        public void ShouldHaveCorrectNumberOfChapters()
        {
            Assert.That(_view.Items.Count(), Is.EqualTo(4));
        }

        [Test]
        public void ShouldHaveCorrectChapterData()
        {
            var actual = _view.Items.FirstOrDefault();
            Assert.That(actual, Is.Not.Null, "Should contain at-least one chapter");
            Assert.That(actual.Title, Is.Not.Empty, "Chapter name should have a value");
            Assert.That(actual.ChapterNumber, Is.GreaterThan(0), "Chapter should have some number.");

            actual.Links.AssertLinkNotPresent("update");
            actual.Links.AssertLinkNotPresent("delete");
        }
    }
}
