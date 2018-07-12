using System;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Book
{
    [TestFixture, Ignore("Security not implemented")]
    public class WhenAddingBookAsReader : IntegrationTestBase
    {
        private BookView _genreView;
        private Domain.Entities.Library.Author _author;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _author = AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "author1" });

            _genreView = new BookView
            {
                Title = "BookName",
                Description = "Some description",
                Language = (int)Languages.Chinese,
                IsPublic = true,
                AuthorId = _author.Id
            };

            Response = await GetReaderClient(Guid.NewGuid()).PostJson($"/api/books", _genreView);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            AuthorDataHelper.Delete(_author.Id);
        }

        [Test]
        public void ShouldReturnUnautorised()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }
    }
}