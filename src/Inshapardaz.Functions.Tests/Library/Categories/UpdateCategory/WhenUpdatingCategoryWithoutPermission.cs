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
    public class WhenUpdatingCategoryWithoutPermission : FunctionTest
    {
        private ForbidResult _response;
        private LibraryDataBuilder _dataBuilder;
        private readonly ClaimsPrincipal _claim;

        public WhenUpdatingCategoryWithoutPermission(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<LibraryDataBuilder>();
            _dataBuilder.Build();
            var handler = Container.GetService<Functions.Library.Categories.UpdateCategory>();
            var faker = new Faker();
            var category = new CategoryView { Id = faker.Random.Number(), Name = faker.Random.String() };
            var request = new RequestBuilder()
                                            .WithJsonBody(category)
                                            .Build();
            _response = (ForbidResult)await handler.Run(request, _dataBuilder.Library.Id, category.Id, _claim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _dataBuilder.CleanUp();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            Assert.That(_response, Is.Not.Null);
        }
    }
}
