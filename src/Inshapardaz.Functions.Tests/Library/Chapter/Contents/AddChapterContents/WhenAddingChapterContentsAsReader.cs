using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.Contents.AddChapterContents
{
    [TestFixture]
    public class WhenAddingChapterContentsAsReader
        : LibraryTest<Functions.Library.Books.Chapters.Contents.AddChapterContents>
    {
        private ForbidResult _response;
        private ChapterDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<ChapterDataBuilder>();

            var chapter = _dataBuilder.AsPublic().Build();
            var contents = new Faker().Random.Words(60);

            var request = new RequestBuilder().WithBody(contents).Build();
            _response = (ForbidResult)await handler.Run(request, LibraryId, chapter.BookId, chapter.Id, AuthenticationBuilder.ReaderClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            Assert.That(_response, Is.Not.Null);
        }
    }
}
