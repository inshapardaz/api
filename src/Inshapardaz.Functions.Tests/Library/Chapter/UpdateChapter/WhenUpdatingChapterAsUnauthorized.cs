using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.UpdateChapter
{
    [TestFixture]
    public class WhenUpdatingChapterAsUnauthorized
        : LibraryTest<Functions.Library.Books.Chapters.UpdateChapter>
    {
        private UnauthorizedResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dataBuilder = Container.GetService<ChapterDataBuilder>();
            var chapters = dataBuilder.WithLibrary(LibraryId).WithContents().Build(4);
            var chapter = chapters.PickRandom();

            var chapter2 = new ChapterView { Title = Random.Name };
            _response = (UnauthorizedResult)await handler.Run(chapter2, LibraryId, chapter.BookId, chapter.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorizedResult()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
