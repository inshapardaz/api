using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.UpdateBook
{
    [TestFixture]
    public class WhenUpdatingBookWithAdditionalCategories : TestBase
    {
        private HttpResponseMessage _response;
        private BookView _expected;
        private BookAssert _bookAssert;
        private List<CategoryDto> _categoriesToUpdate;

        public WhenUpdatingBookWithAdditionalCategories() : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var otherAuthor = AuthorBuilder.WithLibrary(LibraryId).Build();
            var newCategories = CategoryBuilder.WithLibrary(LibraryId).Build(3);
            var otherSeries = SeriesBuilder.WithLibrary(LibraryId).Build();
            var books = BookBuilder.WithLibrary(LibraryId)
                                    .WithCategories(3)
                                    .HavingSeries()
                                    .AddToFavorites(AccountId)
                                    .AddToRecentReads(AccountId)
                                    .Build(1);

            var selectedBook = books.PickRandom();

            _categoriesToUpdate = DatabaseConnection.GetCategoriesByBook(selectedBook.Id).ToList();
            _categoriesToUpdate.AddRange(newCategories);

            var fake = new Faker();
            _expected = new BookView
            {
                Id = selectedBook.Id,
                Title = fake.Name.FullName(),
                Description = fake.Random.Words(5),
                Copyrights = fake.PickRandom<CopyrightStatuses>().ToDescription(),
                Language = Helpers.RandomData.Locale,
                YearPublished = fake.Date.Past().Year,
                Status = fake.PickRandom<StatusType>().ToDescription(),
                IsPublic = fake.Random.Bool(),
                Authors = new List<AuthorView> { new AuthorView { Id = otherAuthor.Id, Name = otherAuthor.Name } },
                SeriesId = otherSeries.Id,
                IsPublished = fake.Random.Bool(),
                Categories = _categoriesToUpdate.Select(c => new CategoryView { Id = c.Id })
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/books/{selectedBook.Id}", _expected);
            _bookAssert = BookAssert.WithResponse(_response).InLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOKResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveUpdatedTheBook()
        {
            _expected.Authors.ElementAt(0).Links = new List<Views.LinkView> { new Views.LinkView {
            Rel = RelTypes.Self,
            Method = "GET",
            Href = $"http://localhost/libraries/{LibraryId}/authors/{_expected.Authors.ElementAt(0).Id}"
            } };
            _bookAssert.ShouldBeSameAs(_expected, DatabaseConnection);
        }

        [Test]
        public void ShouldReturnCorrectCategories()
        {
            _bookAssert.ShouldBeSameCategories(_categoriesToUpdate);
        }

        [Test]
        public void ShouldSaveCorrectCategories()
        {
            _bookAssert.ShouldHaveCategories(_categoriesToUpdate, DatabaseConnection);
        }
    }
}
