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
    public class WhenAddingAuthorAsAnonymousUser : FunctionTest
    {
        private UnauthorizedResult _response;
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
            _response = (UnauthorizedResult)await handler.Run(request, _builder.Library.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _builder.CleanUp();
        }

        [Test]
        public void ShouldHaveUnauthorizedResult()
        {
            Assert.That(_response, Is.Not.Null);
        }
    }
}
