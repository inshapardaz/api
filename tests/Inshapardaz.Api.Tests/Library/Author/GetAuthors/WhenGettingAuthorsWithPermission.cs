using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Author.GetAuthors
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenGettingAuthorsWithPermission : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<AuthorView> _assert;

        public WhenGettingAuthorsWithPermission(Role Role)
            : base(Role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            AuthorBuilder.WithLibrary(LibraryId).WithBooks(3).WithArticles(5).Build(4);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/authors?pageNumber={1}&pageSize={10}");
            _assert = Services.GetService<PagingAssert<AuthorView>>().ForResponse(_response);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/authors");
        }

        [Test]
        public void ShouldHaveCreateLink()
        {
            _assert.ShouldHaveCreateLink($"/libraries/{LibraryId}/authors");
        }

        [Test]
        public void ShouldHaveEditingLinkOnAuthor()
        {
            var expectedItems = AuthorBuilder.Authors;
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                Services.GetService<AuthorAssert>().ForView(actual)
                    .ForLibrary(LibraryId)
                    .ShouldBeSameAs(item)
                    .WithBookCount(3)
                    .WithEditableLinks(CurrentAuthenticationLevel.Value)
                    .ShouldHavePublicImageLink();
            }
        }
    }
}
