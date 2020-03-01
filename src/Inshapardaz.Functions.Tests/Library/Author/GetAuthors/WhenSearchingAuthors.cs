using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Dto;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.GetAuthors
{
    // TODO : Add tests for pagination
    [TestFixture]
    public class WhenSearchingAuthors : LibraryTest
    {
        private AuthorsDataBuilder _builder;
        private OkObjectResult _response;
        private PageView<AuthorView> _view;
        private AuthorDto _searchedAuthor;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<AuthorsDataBuilder>();
            var authors = _builder.WithLibrary(LibraryId).WithBooks(3).Build(20);

            _searchedAuthor = authors.PickRandom();
            var request = new RequestBuilder()
               .WithQueryParameter("query", _searchedAuthor.Name)
               .Build();

            var handler = Container.GetService<Functions.Library.Authors.GetAuthors>();
            _response = (OkObjectResult)await handler.Run(request, LibraryId, AuthenticationBuilder.ReaderClaim, CancellationToken.None);

            _view = _response.Value as PageView<AuthorView>;
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _builder.CleanUp();
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
