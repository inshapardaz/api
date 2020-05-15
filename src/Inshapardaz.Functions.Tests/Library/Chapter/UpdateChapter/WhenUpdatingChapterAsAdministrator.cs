using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.UpdateChapter
{
    [TestFixture]
    public class WhenUpdatingChapterAsAdministrator
        : LibraryTest<Functions.Library.Books.Chapters.UpdateChapter>
    {
        private OkObjectResult _response;
        private ChapterDataBuilder _dataBuilder;
        private ChapterView newChapter;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<ChapterDataBuilder>();

            var chapters = _dataBuilder.WithContents().Build(4);

            var chapter = chapters.First();

            newChapter = new ChapterView { Title = new Faker().Name.FirstName() };
            _response = (OkObjectResult)await handler.Run(newChapter, LibraryId, chapter.BookId, chapter.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);
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
            Assert.That(_response.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public void ShouldHaveUpdatedTheChapter()
        {
            var returned = _response.Value as ChapterView;
            Assert.That(returned, Is.Not.Null);

            var actual = DatabaseConnection.GetChapterById(LibraryId, returned.Id);
            Assert.That(actual.Title, Is.EqualTo(newChapter.Title), "Chapter should have expected title.");
        }
    }
}
