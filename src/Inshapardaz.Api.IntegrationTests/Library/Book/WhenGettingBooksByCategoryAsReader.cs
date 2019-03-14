using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Inshapardaz.Api.IntegrationTests.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Api.IntegrationTests.Library.Book
{
    [TestFixture]
    public class WhenGettingBooksByCategoryAsReader : IntegrationTestBase
    {
        private readonly List<Domain.Entities.Library.Book> _books = new List<Domain.Entities.Library.Book>();
        private Domain.Entities.Library.Category _category1;
        private Domain.Entities.Library.Category _category2;
        private PageView<BookView> _view;
        private Domain.Entities.Library.Author _author;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _category1 = CategoryDataHelper.Create(TestDataProvider.RandomString);
            _category2 = CategoryDataHelper.Create(TestDataProvider.RandomString );
            _author = AuthorDataHelper.Create(new Domain.Entities.Library.Author { Name = TestDataProvider.RandomString });
            _books.Add(BookDataHelper.Create(new Domain.Entities.Library.Book { Title = "book 1", AuthorId = _author.Id, Categories = new[] { _category2 }}));
            _books.Add(BookDataHelper.Create(new Domain.Entities.Library.Book { Title = "book 2", AuthorId = _author.Id, Categories = new[] { _category2 }, IsPublic = true }));
            _books.Add(BookDataHelper.Create(new Domain.Entities.Library.Book { Title = "book 3", AuthorId = _author.Id, Categories = new[] { _category1 }}));
            _books.Add(BookDataHelper.Create(new Domain.Entities.Library.Book { Title = "book 4", AuthorId = _author.Id, Categories = new[] { _category1 }}));

            Response = await GetReaderClient(Guid.NewGuid()).GetAsync($"/api/categories/{_category2.Id}/books");
            _view = JsonConvert.DeserializeObject<PageView<BookView>>(await Response.Content.ReadAsStringAsync());
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            foreach (var book in _books)
            {
                BookDataHelper.Delete(book.Id);
            }

            AuthorDataHelper.Delete(_author.Id);
            CategoryDataHelper.Delete(_category1.Id);
            CategoryDataHelper.Delete(_category2.Id);
        }

        [Test]
        public void ShouldReturnOK()
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
        public void ShouldNotReturnCreateLink()
        {
            _view.Links.ShouldNotContain(link => link.Rel == RelTypes.Create);
        }
    }
}