using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.UpdateAuthor
{
    [TestFixture]
    public class WhenUpdatingAuthorAsWriter : FunctionTest
    {
        private OkObjectResult _response;
        private AuthorsDataBuilder _dataBuilder;
        private AuthorView _expected;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _dataBuilder = Container.GetService<AuthorsDataBuilder>();

            var handler = Container.GetService<Functions.Library.Authors.UpdateAuthor>();
            var authors = _dataBuilder.WithBooks(3).Build(4);

            var selectedAuthor = authors.First();

            _expected = new AuthorView { Name = new Faker().Name.FullName() };
            var request = new RequestBuilder()
                                            .WithJsonBody(_expected)
                                            .Build();
            _response = (OkObjectResult) await handler.Run(request, selectedAuthor.Id, AuthenticationBuilder.WriterClaim, CancellationToken.None);
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
            Assert.That(_response.StatusCode, Is.EqualTo((int) HttpStatusCode.OK));
        }

        [Test]
        public void ShouldHaveUpdatedTheAuthor()
        {
            var returned = _response.Value as AuthorView;
            Assert.That(returned, Is.Not.Null);

            var actual = _dataBuilder.GetById(returned.Id);
            Assert.That(actual.Name, Is.EqualTo(_expected.Name), "Author should have expected name.");
        }
    }
}
