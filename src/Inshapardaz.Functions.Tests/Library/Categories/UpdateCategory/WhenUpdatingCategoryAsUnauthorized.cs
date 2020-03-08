using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Categories.UpdateCategory
{
    [TestFixture]
    public class WhenUpdatingCategoryAsUnauthorized : LibraryTest<Functions.Library.Categories.UpdateCategory>
    {
        private UnauthorizedResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var faker = new Faker();
            var category = new CategoryView { Id = faker.Random.Number(), Name = faker.Random.String() };

            _response = (UnauthorizedResult)await handler.Run(category, LibraryId, category.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            Assert.That(_response, Is.Not.Null);
        }
    }
}
