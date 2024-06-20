using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Articles.UpdateArticle
{
    [TestFixture]
    public class WhenUpdatingArticleWithAdditionalCategories : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleView _expected;
        private ArticleAssert _assert;
        private List<CategoryDto> _categoriesToUpdate;

        public WhenUpdatingArticleWithAdditionalCategories() : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var otherAuthor = AuthorBuilder.WithLibrary(LibraryId).Build();
            var newCategories = CategoryBuilder.WithLibrary(LibraryId).Build(3);
            var otherSeries = SeriesBuilder.WithLibrary(LibraryId).Build();
            var articles = ArticleBuilder.WithLibrary(LibraryId)
                                    .WithCategories(3)
                                    .AddToFavorites(AccountId)
                                    .AddToRecentReads(AccountId)
                                    .Build(2);

            var selectedArticle = articles.PickRandom();

            _categoriesToUpdate = CategoryTestRepository.GetCategoriesByArticle(selectedArticle.Id).ToList();
            _categoriesToUpdate.AddRange(newCategories);

            var fake = new Faker();
            _expected = new ArticleView
            {
                Id = selectedArticle.Id,
                Title = fake.Name.FullName(),
                Status = fake.PickRandom<EditingStatus>().ToDescription(),
                IsPublic = fake.Random.Bool(),
                Authors = new List<AuthorView> { new AuthorView { Id = otherAuthor.Id, Name = otherAuthor.Name } },
                Categories = _categoriesToUpdate.Select(c => new CategoryView { Id = c.Id })
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/articles/{selectedArticle.Id}", _expected);
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
           _assert.ShouldBeSameAs(_expected);
        }

        [Test]
        public void ShouldReturnCorrectCategories()
        {
            _assert.ShouldBeSameCategories(_categoriesToUpdate);
        }

        [Test]
        public void ShouldSaveCorrectCategories()
        {
            _assert.ShouldHaveCategories(_categoriesToUpdate);
        }
    }
}
