using System.Security.Claims;
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
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenUpdatingChapterWithPermission
        : LibraryTest<Functions.Library.Books.Chapters.UpdateChapter>
    {
        private readonly ClaimsPrincipal _claim;
        private OkObjectResult _response;
        private ChapterDataBuilder _dataBuilder;
        private ChapterView newChapter;
        private ChapterAssert _assert;

        public WhenUpdatingChapterWithPermission(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<ChapterDataBuilder>();

            var chapters = _dataBuilder.WithLibrary(LibraryId).Build(4);

            var chapter = chapters.PickRandom();

            newChapter = new ChapterView { Title = Random.Name, ChapterNumber = Random.Number, BookId = chapter.BookId };
            _response = (OkObjectResult)await handler.Run(newChapter, LibraryId, chapter.BookId, chapter.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);
            _assert = ChapterAssert.FromResponse(_response, LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOKResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveReturnedUpdatedChapter()
        {
            _assert.ShouldMatch(newChapter);
        }

        [Test]
        public void ShouldHaveUpdatedChater()
        {
            _assert.ShouldHaveSavedChapter(DatabaseConnection);
        }
    }
}
