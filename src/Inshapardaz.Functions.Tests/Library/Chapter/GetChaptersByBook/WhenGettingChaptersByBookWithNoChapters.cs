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
    public class WhenGettingChaptersByBookWithNoChapters : FunctionTest
    {
        private OkObjectResult _response;
        private ListView<ChapterView> _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            var dataBuilder = Container.GetService<BooksDataBuilder>();
            var book = dataBuilder.Build();

            var handler = Container.GetService<Functions.Library.Books.Chapters.GetChaptersByBook>();
            _response = (OkObjectResult) await handler.Run(null, book.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);

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
            _view.Links.AssertLink("create")
                        .ShouldBePost()
                        .ShouldHaveSomeHref();
        }

         [Test]
        public void ShouldHaveNoChapters()
        {
            Assert.That(_view.Items, Is.Not.Null);
            Assert.That(_view.Items.Count(), Is.Zero);
        }
    }
}
