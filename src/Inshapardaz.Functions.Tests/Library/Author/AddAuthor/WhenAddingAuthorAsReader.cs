using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.AddAuthor
{
    [TestFixture]
    public class WhenAddingAuthorAsReader : FunctionTest
    {
        private ForbidResult _response;
        private LibraryDataBuilder _builder;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<LibraryDataBuilder>();
            _builder.Build();

            var handler = Container.GetService<Functions.Library.Authors.AddAuthor>();
            var author = new AuthorView { Name = new Faker().Random.String() };

            var request = new RequestBuilder()
                                            .WithJsonBody(author)
                                            .Build();
            _response = (ForbidResult)await handler.Run(request, _builder.Library.Id, AuthenticationBuilder.ReaderClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _builder.CleanUp();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            Assert.That(_response, Is.Not.Null);
        }
    }
}
