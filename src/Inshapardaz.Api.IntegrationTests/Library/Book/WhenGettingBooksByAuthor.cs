using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Book
{
    [TestFixture]
    public class WhenGettingBooksByAuthor : IntegrationTestBase
    {
        private readonly List<Domain.Entities.Library.Book> _books = new List<Domain.Entities.Library.Book>();
        private PageView<BookView> _view;
        private Domain.Entities.Library.Author _author1;
        private Domain.Entities.Library.Author _author2;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _author1 = AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "author1" });
            _author2 = AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = "author2" });

            _books.Add(BookDataHelper.Create(new Domain.Entities.Library.Book { Title = "book 1", AuthorId = _author1.Id }));
            _books.Add(BookDataHelper.Create(new Domain.Entities.Library.Book { Title = "book 2", AuthorId = _author1.Id }));
            _books.Add(BookDataHelper.Create(new Domain.Entities.Library.Book { Title = "book 3", AuthorId = _author2.Id }));
            _books.Add(BookDataHelper.Create(new Domain.Entities.Library.Book { Title = "book 4", AuthorId = _author2.Id }));

             Response = await GetAdminClient(Guid.NewGuid()).GetAsync($"/api/authors/{_author2.Id}/books");
            _view = JsonConvert.DeserializeObject<PageView<BookView>>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            foreach (var book in _books)
            {
                BookDataHelper.Delete(book.Id);
            }

            AuthorDataHelper.Delete(_author1.Id);
            AuthorDataHelper.Delete(_author2.Id);
        }

        [Test]
        public void ShouldReturnOk()
        {
            Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Test]
        public void ShouldContainAllBook()
        {
            _view.Data.Count().ShouldBe(2);
        }

        [Test]
        public void ShouldReturnSelfLink()
        {
            _view.Links.ShouldContain(link => link.Rel == RelTypes.Self);
        }

        [Test]
        public void ShouldReturnCreateLink()
        {
            _view.Links.ShouldContain(link => link.Rel == RelTypes.Create);
        }
    }
}