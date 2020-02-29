using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.GetAuthorById
{
    [TestFixture]
    public class WhenGettingAuthorByIdThatDoesNotExist : FunctionTest
    {
        private LibraryDataBuilder _builder;

        private NotFoundResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<LibraryDataBuilder>();
            _builder.Build();

            var request = TestHelpers.CreateGetRequest();

            var handler = Container.GetService<Functions.Library.Authors.GetAuthorById>();
            _response = (NotFoundResult)await handler.Run(request, _builder.Library.Id, Random.Number, AuthenticationBuilder.WriterClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _builder.CleanUp();
        }

        [Test]
        public void ShouldHaveNotFoundResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }
    }
}
