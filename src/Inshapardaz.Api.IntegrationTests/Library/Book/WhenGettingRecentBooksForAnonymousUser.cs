using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Book
{
    [TestFixture, Ignore("Security not implemented")]
    public class WhenGettingRecentBooksForAnonymousUser : IntegrationTestBase
    {
        private readonly List<Domain.Entities.Library.Book> _books = new List<Domain.Entities.Library.Book>();
        private readonly List<Domain.Entities.Library.Book> _recents = new List<Domain.Entities.Library.Book>();

        private readonly Guid UserId = Guid.NewGuid();
        private Domain.Entities.Library.Author _author;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _author = AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "author1" });

            _books.Add(BookDataHelper.Create(new Domain.Entities.Library.Book { Title = "book 1", AuthorId = _author.Id }));
            _books.Add(BookDataHelper.Create(new Domain.Entities.Library.Book { Title = "book 2", AuthorId = _author.Id }));
            _books.Add(BookDataHelper.Create(new Domain.Entities.Library.Book { Title = "book 3", AuthorId = _author.Id }));
            _books.Add(BookDataHelper.Create(new Domain.Entities.Library.Book { Title = "book 4", AuthorId = _author.Id }));

            _recents.Add(BookDataHelper.AddRecent(UserId, _books[0]));
            _recents.Add(BookDataHelper.AddRecent(UserId, _books[3]));
            
            Response = await GetClient().GetAsync("/api/books/recent");
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            foreach (var book in _books)
            {
                BookDataHelper.Delete(book.Id);
            }

            foreach (var book in _recents)
            {
                BookDataHelper.RemoveFromRecent(UserId, book);
            }

            AuthorDataHelper.Delete(_author.Id);
        }

        [Test]
        public void ShouldReturnNotFound()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
    }
}