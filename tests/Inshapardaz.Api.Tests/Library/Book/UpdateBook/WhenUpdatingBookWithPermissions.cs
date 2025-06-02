using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.UpdateBook
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenUpdatingBookWithPermissions : TestBase
    {
        private HttpResponseMessage _response;
        private BookView _expected;
        private BookAssert _bookAssert;
        private IEnumerable<CategoryDto> _otherCategories;
        private List<TagView> _newTags;

        public WhenUpdatingBookWithPermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var otherAuthor = AuthorBuilder.WithLibrary(LibraryId).Build();
            _otherCategories = CategoryBuilder.WithLibrary(LibraryId).Build(3);
            var otherSeries = SeriesBuilder.WithLibrary(LibraryId).Build();
            var books = BookBuilder.WithLibrary(LibraryId)
                                    .WithCategories(1)
                                    .HavingSeries()
                                    .AddToFavorites(AccountId)
                                    .AddToRecentReads(AccountId)
                                    .WithTags(3)
                                    .Build(1);

            var selectedBook = books.PickRandom();

            _newTags = new List<TagView>()
            {
                new TagView { Name = "tag1" },
                new TagView { Name = "Tag2" },
            };
            
            var fake = new Faker();
            _expected = new BookView
            {
                Id = selectedBook.Id,
                Title = fake.Name.FullName(),
                Description = fake.Random.Words(5),
                Copyrights = fake.PickRandom<CopyrightStatuses>().ToDescription(),
                Language = Framework.Helpers.RandomData.Locale,
                YearPublished = fake.Date.Past().Year,
                Status = fake.PickRandom<StatusType>().ToDescription(),
                IsPublic = fake.Random.Bool(),
                Authors = new List<AuthorView> { new AuthorView { Id = otherAuthor.Id, Name = otherAuthor.Name } },
                SeriesId = otherSeries.Id,
                IsPublished = fake.Random.Bool(),
                Source = RandomData.String,
                Publisher = RandomData.String,
                Categories = _otherCategories.Select(c => new CategoryView { Id = c.Id }),
                Tags = _newTags
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/books/{selectedBook.Id}", _expected);
            _bookAssert = Services.GetService<BookAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
            _bookAssert.ShouldBeSameAs(_expected);
        }

        [Test]
        public void ShouldReturnCorrectCategories()
        {
            _bookAssert.ShouldBeSameCategories(_otherCategories);
        }
        
        [Test]
        public void ShouldReturnCorrectTags()
        {
            _bookAssert.ShouldHaveMatchingTags(_newTags);
        }
    }
}
