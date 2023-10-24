using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.UpdateArticle
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenUpdatingArticleWithPermissions : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleView _expected;
        private ArticleAssert _assert;
        private IEnumerable<CategoryDto> _otherCategories;

        public WhenUpdatingArticleWithPermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var otherAuthors = AuthorBuilder.WithLibrary(LibraryId).Build(2);
            _otherCategories = CategoryBuilder.WithLibrary(LibraryId).Build(3);
            var article = ArticleBuilder.WithLibrary(LibraryId)
                                    .WithCategories(1)
                                    .AddToFavorites(AccountId)
                                    .AddToRecentReads(AccountId)
                                    .Build(1)
                                    .Single();

            var fake = new Faker();
            _expected = new ArticleView
            {
                Id = article.Id,
                Title = fake.Name.FullName(),
                Status = fake.PickRandom<EditingStatus>().ToDescription(),
                IsPublic = fake.Random.Bool(),
                Authors = otherAuthors.Select(x => new AuthorView { Id = x.Id, Name = x.Name }),
                Categories = _otherCategories.Select(c => new CategoryView { Id = c.Id }),
                WriterAccountId = AccountId,
                WriterAssignTimeStamp = DateTime.UtcNow,
                ReviewerAccountId = AccountId,
                ReviewerAssignTimeStamp = DateTime.UtcNow,
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/articles/{article.Id}", _expected);
            _assert = ArticleAssert.FromResponse(_response, LibraryId);
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
            _assert.ShouldBeSameAs(_expected, DatabaseConnection);
        }

        [Test]
        public void ShouldReturnCorrectCategories()
        {
            _assert.ShouldBeSameCategories(_otherCategories.ToList());
        }
    }
}
