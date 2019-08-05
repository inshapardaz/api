using System.Linq;
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
    public class WhenUpdatingChapterAsUnauthorized: FunctionTest
    {
        UnauthorizedResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dataBuilder = Container.GetService<ChapterDataBuilder>();
            var chapters = dataBuilder.WithChapters(4).Build();
            var chapter = chapters.First();

            var handler = Container.GetService<Functions.Library.Books.Chapters.UpdateChapter>();
            var faker = new Faker();
            var request = new ChapterView { Title = faker.Random.String() };
            _response = (UnauthorizedResult)await handler.Run(request, NullLogger.Instance, chapter.BookId, chapter.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorizedResult()
        {
            Assert.That(_response, Is.Not.Null);
        }
    }
}
