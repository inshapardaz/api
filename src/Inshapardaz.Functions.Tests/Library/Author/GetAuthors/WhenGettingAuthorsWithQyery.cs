using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.GetAuthors
{
    [TestFixture]
    public class WhenGettingAuthorsWithQyery : FunctionTest
    {
        private OkObjectResult _response;
        private PageView<AuthorView> _view;
        private Ports.Database.Entities.Library.Author _searchedAuthor;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var builder = Container.GetService<AuthorsDataBuilder>();
            var authors = builder.WithBooks(3).Build(20);

            _searchedAuthor = authors.Last();
            var request = new RequestBuilder()
               .WithQueryParameter("query", _searchedAuthor.Name)
               .Build();

            var handler = Container.GetService<Functions.Library.Authors.GetAuthors>();
            _response = (OkObjectResult) await handler.Run(request, NullLogger.Instance, AuthenticationBuilder.ReaderClaim, CancellationToken.None);

            _view = _response.Value as PageView<AuthorView>;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void ShouldReturnSearchedAuthor()
        {
            Assert.That(_view.Data.Count(), Is.EqualTo(1));
            Assert.That(_view.Data.First().Id, Is.EqualTo(_searchedAuthor.Id));
        }
    }
}
