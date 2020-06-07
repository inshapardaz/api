using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.GetAuthors
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenGettingAuthorsWithPermission : LibraryTest<Functions.Library.Authors.GetAuthors>
    {
        private AuthorsDataBuilder _builder;
        private OkObjectResult _response;
        private PagingAssert<AuthorView> _assert;
        private readonly ClaimsPrincipal _claim;

        public WhenGettingAuthorsWithPermission(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            _builder = Container.GetService<AuthorsDataBuilder>();
            _builder.WithLibrary(LibraryId).WithBooks(3).Build(4);

            _response = (OkObjectResult)await handler.Run(request, LibraryId, _claim, CancellationToken.None);

            _assert = new PagingAssert<AuthorView>(_response, Library);
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
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _assert.ShouldHaveSelfLink($"/api/library/{LibraryId}/authors");
        }

        [Test]
        public void ShouldHaveCreateLink()
        {
            _assert.ShouldHaveCreateLink($"/api/library/{LibraryId}/authors");
        }

        [Test]
        public void ShouldHaveEditingLinkOnAuthor()
        {
            var expectedItems = _builder.Authors;
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                actual.ShouldMatch(item)
                      .InLibrary(LibraryId)
                      .WithBookCount(3)
                      .WithEditableLinks()
                      .ShouldHavePublicImageLink();
            }
        }
    }
}
