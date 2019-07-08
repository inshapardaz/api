using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.UpdateAuthor
{
    [TestFixture]
    public class WhenUpdatingAuthorThatDoesNotExist : FunctionTest
    {
        CreatedResult _response;
        private AuthorsDataBuilder _builder;
        private AuthorView _expected;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<AuthorsDataBuilder>();

            var handler = Container.GetService<Functions.Library.Authors.UpdateAuthor>();
            var faker = new Faker();
            _expected = new AuthorView { Name = new Faker().Random.String() };
            _response = (CreatedResult) await handler.Run(_expected, NullLogger.Instance, _expected.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);
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
        public void ShouldHaveCreatedTheAuthor()
        {
            var returned = _response.Value as AuthorView;
            Assert.That(returned, Is.Not.Null);

            var actual = _builder.GetById(returned.Id);
            Assert.That(actual, Is.Not.Null, "Author should be created.");
        }
    }
}
