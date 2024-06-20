using System;
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

namespace Inshapardaz.Api.Tests.Library.Articles.UpdateArticle
{
    [TestFixture]
    public class WhenUpdatingArticleWithMultipleAuthors : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleView _expected;
        private ArticleAssert _assert;
        private IEnumerable<CategoryDto> _otherCategories;

        public WhenUpdatingArticleWithMultipleAuthors()
            : base(Role.LibraryAdmin)
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
                                    .First();

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
            _assert = Services.GetService<ArticleAssert>().ForLibrary(LibraryId).ForResponse(_response);
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
        public void ShouldHaveUpdatedTheArticle()
        {
            _expected.Authors.ElementAt(0).Links = new List<Views.LinkView> { new Views.LinkView {
            Rel = RelTypes.Self,
            Method = "GET",
            Href = $"http://localhost/libraries/{LibraryId}/authors/{_expected.Authors.ElementAt(0).Id}"
            } };
            _assert.ShouldBeSameAs(_expected);
        }

        [Test]
        public void ShouldReturnCorrectCategories()
        {
            _assert.ShouldBeSameCategories(_otherCategories.ToList());
        }
    }
}
