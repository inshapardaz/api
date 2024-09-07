using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.PublishBook
{
    [TestFixture]
    public class WhenPublishingBook : TestBase
    {
        private BookDto _book;
        private HttpResponseMessage _response;
        private PagingAssert<BookPageView> _assert;

        public WhenPublishingBook()
            : base(Role.LibraryAdmin)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _book = BookBuilder.WithLibrary(LibraryId).WithChapters(3).WithPages(13).Build();

            _response = await Client.PostAsync($"/libraries/{LibraryId}/books/{_book.Id}/publish", null);

            _assert = Services.GetService<PagingAssert<BookPageView>>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveUpdatedChapters()
        {
            var bookPageTestRepository = Services.GetService<IBookPageTestRepository>();
            var pages = bookPageTestRepository.GetBookPages(_book.Id);

            var fileRepository = Services.GetService<IFileTestRepository>();
            var fileStorage = Services.GetService<FakeFileStorage>();

            var expectedChapterContents = pages.Select(page =>
                {
                    var file = fileRepository.GetFileById(page.ContentId.Value);
                    var content = fileStorage.GetTextFile(file.FilePath, CancellationToken.None).Result;

                    return new { ChapterId = page.ChapterId, SequenceNumber = page.SequenceNumber, Content = content };
                })
                .GroupBy(p => p.ChapterId)
                .Select(group => new
                {
                    ChapterId = group.Key,
                    Content = string.Join(' ', group.OrderBy(x => x.SequenceNumber).Select(x => x.Content).ToList())
                }).ToList();
            
            var chapterTestRepository = Services.GetService<IChapterTestRepository>();
            var chapters = chapterTestRepository.GetChaptersByBook(_book.Id);

            foreach (var chapter in chapters)
            {
                var chapterContent = chapterTestRepository.GetContentByChapter(chapter.Id);
                var file = fileRepository.GetFileById(chapterContent.Single().FileId);
                var content = fileStorage.GetTextFile(file.FilePath, CancellationToken.None).Result;
                var expectedChapterContent = expectedChapterContents.SingleOrDefault(x => x.ChapterId == chapter.Id);
                expectedChapterContent.Should().NotBeNull();
                expectedChapterContent.Content.Should().Be(content);
            }
        }
    }
}
