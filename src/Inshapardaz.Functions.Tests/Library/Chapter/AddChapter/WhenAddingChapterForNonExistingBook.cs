using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.AddChapter
{
    [TestFixture]
    public class WhenAddingChapterForNonExistingBook
        : LibraryTest<Functions.Library.Books.Chapters.AddChapter>
    {
        private BadRequestResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var chapter = new ChapterView { Title = Random.Name, ChapterNumber = 1 };

            _response = (BadRequestResult)await handler.Run(chapter, LibraryId, Random.Number, AuthenticationBuilder.WriterClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveBadRequestResult()
        {
            _response.ShouldBeBadRequest();
        }
    }
}
