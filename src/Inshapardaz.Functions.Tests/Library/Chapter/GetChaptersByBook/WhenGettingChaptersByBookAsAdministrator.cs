using System.Collections.Generic;
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
    public class WhenGettingChaptersByBookAsAdministrator : FunctionTest
    {
        OkObjectResult _response;
        ListView<ChapterView> _view;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            var dataBuilder = Container.GetService<ChapterDataBuilder>();
            var chapters = dataBuilder.WithContents().AsPublic().Build(4);
            var book = chapters.First().Book;

            var handler = Container.GetService<Functions.Library.Books.Chapters.GetChaptersByBook>();
            _response = (OkObjectResult) await handler.Run(null, book.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);

            _view = _response.Value as ListView<ChapterView>;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
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
        public void ShouldNotHaveCreateLink()
        {
            _view.Links.AssertLink("create")
                        .ShouldBePost()
                        .ShouldHaveSomeHref();
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

            actual.Links.AssertLink("update")
                        .ShouldBePut()
                        .ShouldHaveSomeHref();
            actual.Links.AssertLink("delete")
                        .ShouldBeDelete()
                        .ShouldHaveSomeHref();
            actual.Links.AssertLink("book")
                        .ShouldBeGet()
                        .ShouldHaveSomeHref();
            actual.Links.AssertLink("contents")
                        .ShouldBeGet()
                        .ShouldHaveSomeHref();
            actual.Links.AssertLink("update-contents")
                        .ShouldBePut()
                        .ShouldHaveSomeHref();
            actual.Links.AssertLink("delete-contents")
                        .ShouldBeDelete()
                        .ShouldHaveSomeHref();
        }
    }
}
