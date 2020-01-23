using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Ports.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.DeleteChapter
{
    [TestFixture]
    public class WhenDeletingChapterAsAdministrator : FunctionTest
    {
        private NoContentResult _response;

        private Ports.Database.Entities.Library.Chapter _expected;
        private ChapterDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            _dataBuilder = Container.GetService<ChapterDataBuilder>();
            _expected = _dataBuilder.WithContents().Build();
            
            var handler = Container.GetService<Functions.Library.Books.Chapters.DeleteChapter>();
            _response = (NoContentResult) await handler.Run(request, _expected.BookId, _expected.Id, AuthenticationBuilder.AdminClaim, NullLogger.Instance, CancellationToken.None);
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
            var cat = _dataBuilder.GetById(_expected.Id);
            Assert.That(cat, Is.Null, "Chapter should be deleted.");
        }


        [Test]
        public void ShouldHaveDeletedTheChapterContents()
        { 
            var db = Container.GetService<IDatabaseContext>();
            foreach (var chapterContent in _expected.Contents)
            {
                var file = db.ChapterContent.Where(i => i.Id == chapterContent.Id);
                Assert.That(file, Is.Empty, "Chapter Contents should be deleted");
            }
        }
    }
}
