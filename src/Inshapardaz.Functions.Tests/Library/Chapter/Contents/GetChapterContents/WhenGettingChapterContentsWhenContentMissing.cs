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
        private NotFoundResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dataBuilder = Container.GetService<ChapterDataBuilder>();
            var fileStore = Container.GetService<IFileStorage>() as FakeFileStorage;

            var faker = new Faker();
            var contentUrl = faker.Internet.Url();
            var chapter = dataBuilder.WithContentLink(contentUrl).WithContents().AsPublic().Build();
            
            var handler = Container.GetService<Functions.Library.Books.Chapters.Contents.GetChapterContents>();
            var actionResult = await handler.Run(null, chapter.BookId, chapter.Id, AuthenticationBuilder.ReaderClaim, CancellationToken.None);
            _response = (NotFoundResult) actionResult;
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
