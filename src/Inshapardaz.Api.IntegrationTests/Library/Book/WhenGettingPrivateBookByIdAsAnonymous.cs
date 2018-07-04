using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Book
{
    [TestFixture, Ignore("Security not implemented")]
    public class WhenGettingPrivateBookByIdAsAnonymous : IntegrationTestBase
    {
        private Domain.Entities.Library.Author _author;
        private Domain.Entities.Library.Book _book;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _author = AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "author1" });

            _book = new Domain.Entities.Library.Book
            {
                Title = "BookName",
                Description = "Some description",
                Language = Languages.Chinese,
                IsPublic = false,
                AuthorId = _author.Id
            };

            Response = await GetClient().GetAsync($"/api/books/{_book.Id}");
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            BookDataHelper.Delete(_book.Id);
        }

        [Test]
        public void ShouldReturnUnauthorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
    }
}