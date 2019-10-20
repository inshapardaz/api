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

namespace Inshapardaz.Functions.Tests.Library.Chapter.AddChapter
{
    [TestFixture]
    public class WhenAddingChapterAsWriter : FunctionTest
    {
        private CreatedResult _response;
        private BooksDataBuilder _builder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<BooksDataBuilder>();

            var book = _builder.Build();

            var handler = Container.GetService<Functions.Library.Books.Chapters.AddChapter>();
            var chapter = new ChapterView { Title = new Faker().Random.String(), ChapterNumber = 1 };
            var request = new RequestBuilder()
                                            .WithJsonBody(chapter)
                                            .Build();
            _response = (CreatedResult)await handler.Run(request, book.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo((int)HttpStatusCode.Created));
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            Assert.That(_response.Location, Is.Not.Empty);
        }

        [Test]
        public void ShouldHaveCreatedTheChapter()
        {
            var actual = _response.Value as ChapterView;
            Assert.That(actual, Is.Not.Null);

            var cat = _builder.GetById(actual.Id);
            Assert.That(cat, Is.Not.Null, "Chapter should be created.");
        }
    }
}
