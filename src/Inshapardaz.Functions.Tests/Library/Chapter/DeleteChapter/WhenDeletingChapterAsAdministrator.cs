using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.DeleteChapter
{
    [TestFixture]
    public class WhenDeletingChapterAsAdministrator
        : LibraryTest<Functions.Library.Books.Chapters.DeleteChapter>
    {
        private NoContentResult _response;

        private ChapterDto _expected;
        private ChapterDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            _dataBuilder = Container.GetService<ChapterDataBuilder>();
            _expected = _dataBuilder.WithContents().Build();

            _response = (NoContentResult)await handler.Run(request, LibraryId, _expected.BookId, _expected.Id, AuthenticationBuilder.AdminClaim, NullLogger.Instance, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo(204));
        }

        [Test]
        public void ShouldHaveDeletedChapter()
        {
            var cat = DatabaseConnection.GetChapterById(LibraryId, _expected.Id);
            Assert.That(cat, Is.Null, "Chapter should be deleted.");
        }

        [Test]
        public void ShouldHaveDeletedTheChapterContents()
        {
            Check.ThatFilesAreDeletedForChapter(_expected.Id);
        }
    }
}
