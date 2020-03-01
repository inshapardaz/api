using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Categories.UpdateCategory
{
    [TestFixture]
    public class WhenUpdatingCategoryThatDoesNotExist : LibraryTest
    {
        private CreatedResult _response;
        private CategoryView _expectedCategory;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var handler = Container.GetService<Functions.Library.Categories.UpdateCategory>();
            var faker = new Faker();
            _expectedCategory = new CategoryView { Id = faker.Random.Number(), Name = faker.Random.String() };
            var request = new RequestBuilder()
                                            .WithJsonBody(_expectedCategory)
                                            .Build();
            _response = (CreatedResult)await handler.Run(request, LibraryId, _expectedCategory.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo((int)HttpStatusCode.Created));
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            Assert.That(_response.Location, Is.Not.Empty);
        }

        [Test]
        public void ShouldHaveCreatedTheCategory()
        {
            var createdCategory = _response.Value as CategoryView;
            Assert.That(createdCategory, Is.Not.Null);

            var cat = DatabaseConnection.GetCategoryById(createdCategory.Id);
            Assert.That(cat, Is.Not.Null, "Category should be created.");
        }
    }
}
