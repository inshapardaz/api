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
    public class WhenUpdatingChapterThatDoesNotExist
        : LibraryTest<Functions.Library.Books.Chapters.UpdateChapter>
    {
        private CreatedResult _response;
        private ChapterView newChapter;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var bookBuilder = Container.GetService<BooksDataBuilder>();
            var book = bookBuilder.Build();

            newChapter = new ChapterView { Title = new Faker().Random.String() };
            _response = (CreatedResult)await handler.Run(newChapter, LibraryId, book.Id, newChapter.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);
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
            var returned = _response.Value as ChapterView;
            Assert.That(returned, Is.Not.Null);

            var actual = DatabaseConnection.GetChapterById(LibraryId, returned.Id);
            Assert.That(actual, Is.Not.Null, "Chapter should be created.");
        }
    }
}
