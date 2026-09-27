using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Categories.GetCategoryTree
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    [TestFixture(Role.Reader)]
    public class WhenGettingCategoryTree(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;
        private ListView<CategoryView> _view;
        private CategoryDto _root;
        private CategoryDto _otherRoot;
        private List<CategoryDto> _children;
        private CategoryDto _grandChild;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _root = CategoryBuilder.WithLibrary(LibraryId).Build();
            _children = CategoryBuilder.WithParent(_root).Build(2).ToList();
            _grandChild = CategoryBuilder.WithParent(_children[0]).Build();
            _otherRoot = CategoryBuilder.WithParent(null).Build();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/categories/tree");
            _view = await _response.GetContent<ListView<CategoryView>>();
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldReturnOk() => _response.ShouldBeOk();

        [Test]
        public void ShouldHaveSelfLink()
        {
            _view.SelfLink()
                .ShouldBeGet()
                .EndingWith($"/libraries/{LibraryId}/categories");
        }

        [Test]
        public void ShouldOnlyReturnRootCategoriesAtTopLevel()
        {
            var ids = _view.Data.Select(c => c.Id).ToList();
            ids.Should().Contain(new[] { _root.Id, _otherRoot.Id });
            ids.Should().NotContain(_children.Select(c => c.Id));
            ids.Should().NotContain(_grandChild.Id);
        }

        [Test]
        public void ShouldNestChildrenUnderTheirParent()
        {
            var root = _view.Data.Single(c => c.Id == _root.Id);
            root.Children.Select(c => c.Id).Should().BeEquivalentTo(_children.Select(c => c.Id));
        }

        [Test]
        public void ShouldNestGrandChildrenUnderTheirParent()
        {
            var root = _view.Data.Single(c => c.Id == _root.Id);
            var child = root.Children.Single(c => c.Id == _children[0].Id);
            child.Children.Should().ContainSingle(c => c.Id == _grandChild.Id);
        }

        [Test]
        public void ShouldNotHaveChildrenForLeafCategories()
        {
            var other = _view.Data.Single(c => c.Id == _otherRoot.Id);
            other.Children.Should().BeNullOrEmpty();
        }
    }
}
