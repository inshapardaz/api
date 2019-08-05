using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Fakes;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.Contents.GetChapterContents
{
    [TestFixture]
    public class WhenGettingChapterContentsWhenContentMissing : FunctionTest
    {
        NotFoundResult _response;

        private Ports.Database.Entities.Library.ChapterContent _expected;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            var dataBuilder = Container.GetService<ChapterDataBuilder>();
            var fileStore = Container.GetService<IFileStorage>() as FakeFileStorage;

            var faker = new Faker();
            var contentUrl = faker.Internet.Url();
            var chapters = dataBuilder.WithContentLink(contentUrl).WithChapters(1, true, true).Build();
            var chapter = chapters.First();
            
            var handler = Container.GetService<Functions.Library.Books.Chapters.Contents.GetChapterContents>();
            _response = (NotFoundResult) await handler.Run(null, chapter.BookId, chapter.Id, AuthenticationBuilder.ReaderClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNotFoundResult()
        {
            Assert.That(_response, Is.Not.Null);
        }
    }
}
