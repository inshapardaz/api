using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.GetChildCategories
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.Reader)]
    public class WhenGettingChildCategories(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;
        private ListView<CategoryView> _view;
        private CategoryDto _parent;
        private List<CategoryDto> _children;
        private CategoryDto _grandChild;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _parent = CategoryBuilder.WithLibrary(LibraryId).Build();
            _children = CategoryBuilder.WithParent(_parent).Build(3).ToList();
            _grandChild = CategoryBuilder.WithParent(_children[0]).Build();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/categories/{_parent.Id}/children");
            _view = await _response.GetContent<ListView<CategoryView>>();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldReturnOnlyDirectChildren()
        {
            _view.Data.Select(c => c.Id).Should().BeEquivalentTo(_children.Select(c => c.Id));
        }

        [Test]
        public void ShouldNotReturnGrandChildren() => _view.Data.Should().NotContain(c => c.Id == _grandChild.Id);

        [Test]
        public void ShouldReturnChildrenWithParentDetails()
        {
            foreach (var child in _children)
            {
                var actual = _view.Data.Single(c => c.Id == child.Id);
                Services.GetService<CategoryAssert>().ForView(actual)
                    .ForLibrary(LibraryId)
                    .ShouldBeSameAs(child)
                    .ShouldHaveParent(_parent.Id)
                    .ShouldHaveParentLink(_parent.Id)
                    .ShouldHaveChildrenLink();
            }
        }

        [Test]
        public void ShouldReportChildCount()
        {
            var actual = _view.Data.Single(c => c.Id == _children[0].Id);
            actual.ChildCount.Should().Be(1);
        }
    }
}
