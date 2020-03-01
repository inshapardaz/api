using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Categories.UpdateCategory
{
    [TestFixture(AuthenticationLevel.Reader)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenUpdatingCategoryWithoutPermission : LibraryTest
    {
        private ForbidResult _response;
        private readonly ClaimsPrincipal _claim;

        public WhenUpdatingCategoryWithoutPermission(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var handler = Container.GetService<Functions.Library.Categories.UpdateCategory>();
            var faker = new Faker();
            var category = new CategoryView { Id = faker.Random.Number(), Name = faker.Random.String() };
            var request = new RequestBuilder()
                                            .WithJsonBody(category)
                                            .Build();
            _response = (ForbidResult)await handler.Run(request, LibraryId, category.Id, _claim, CancellationToken.None);
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
