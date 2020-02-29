using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Categories.AddCategory
{
    [TestFixture]
    public class WhenAddingCategoryAsUnauthorized : FunctionTest
    {
        private UnauthorizedResult _response;
        private LibraryDataBuilder _dataBuilder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<LibraryDataBuilder>();
            _dataBuilder.Build();

            var handler = Container.GetService<Functions.Library.Categories.AddCategory>();
            var category = new CategoryView { Name = new Faker().Random.String() };
            var request = new RequestBuilder()
                                            .WithJsonBody(category)
                                            .Build();
            _response = (UnauthorizedResult)await handler.Run(request, _dataBuilder.Library.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
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
