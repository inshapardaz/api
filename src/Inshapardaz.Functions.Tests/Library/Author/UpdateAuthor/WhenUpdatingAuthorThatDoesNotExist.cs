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

namespace Inshapardaz.Functions.Tests.Library.Author.UpdateAuthor
{
    [TestFixture]
    public class WhenUpdatingAuthorThatDoesNotExist : FunctionTest
    {
        private CreatedResult _response;
        private LibraryDataBuilder _builder;
        private AuthorView _expected;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<LibraryDataBuilder>();
            _builder.Build();

            var handler = Container.GetService<Functions.Library.Authors.UpdateAuthor>();
            _expected = new AuthorView { Name = new Faker().Random.String() };

            var request = new RequestBuilder()
                                            .WithJsonBody(_expected)
                                            .Build();

            _response = (CreatedResult)await handler.Run(request, _builder.Library.Id, _expected.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _builder.CleanUp();
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
        public void ShouldHaveCreatedTheAuthor()
        {
            var returned = _response.Value as AuthorView;
            Assert.That(returned, Is.Not.Null);

            var actual = DatabaseConnection.GetAuthorById(returned.Id);
            Assert.That(actual, Is.Not.Null, "Author should be created.");
        }
    }
}
