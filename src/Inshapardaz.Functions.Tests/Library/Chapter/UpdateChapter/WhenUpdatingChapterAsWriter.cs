using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.UpdateChapter
{
    [TestFixture]
    public class WhenUpdatingChapterAsWriter : FunctionTest
    {
        OkObjectResult _response;
        private ChapterDataBuilder _dataBuilder;
        private ChapterView _expected;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<ChapterDataBuilder>();

            var handler = Container.GetService<Functions.Library.Books.Chapters.UpdateChapter>();
            var chapters = _dataBuilder.WithChapters(4).Build();

            var chapter = chapters.First();

            _expected = new ChapterView { Title = new Faker().Random.String() };
            _response = (OkObjectResult) await handler.Run(_expected, NullLogger.Instance, chapter.BookId, chapter.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOKResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo((int) HttpStatusCode.OK));
        }

        [Test]
        public void ShouldHaveUpdatedTheChapter()
        {
            var returned = _response.Value as ChapterView;
            Assert.That(returned, Is.Not.Null);

            var actual = _dataBuilder.GetById(returned.Id);
            Assert.That(actual.Title, Is.EqualTo(_expected.Title), "Chapter should have expected title.");
        }
    }
}
