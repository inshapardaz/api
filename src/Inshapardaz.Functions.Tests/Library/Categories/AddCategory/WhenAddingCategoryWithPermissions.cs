using System.Net;
using System.Security.Claims;
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

namespace Inshapardaz.Functions.Tests.Library.Categories.AddCategory
{
    [TestFixture]
    public class WhenAddingCategoryWithPermissions : FunctionTest
    {
        private CreatedResult _response;
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
            _response = (CreatedResult)await handler.Run(request, _dataBuilder.Library.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _dataBuilder.CleanUp();
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
