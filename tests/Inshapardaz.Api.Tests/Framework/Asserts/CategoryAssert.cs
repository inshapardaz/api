using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using System.Net.Http;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class CategoryAssert
    {
        private HttpResponseMessage _response;
        private CategoryView _category;
        private int _libraryId;

        private readonly ICategoryTestRepository _categoryRepository;
        private readonly IFileTestRepository _fileRepository;

        public CategoryAssert(IFileTestRepository fileRepository,
            ICategoryTestRepository categoryRepository)
        {
            _fileRepository = fileRepository;
            _categoryRepository = categoryRepository;
        }

        public CategoryAssert ForView(CategoryView view)
        {
            _category = view;
            return this;
        }

        public CategoryAssert ForResponse(HttpResponseMessage response)
        {
            _response = response;
            _category = response.GetContent<CategoryView>().Result;
            return this;
        }

        public CategoryAssert ForLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public CategoryAssert ShouldNotHaveDeletedCategory(int categoryId)
        {
            var author = _categoryRepository.GetCategoryById(_libraryId, categoryId);
            author.Should().NotBeNull();
            return this;
        }

        public CategoryAssert ShouldHaveDeletedCategory(int categoryId)
        {
            var author = _categoryRepository.GetCategoryById(_libraryId, categoryId);
            author.Should().BeNull();
            return this;
        }

        public CategoryAssert ShouldHaveCorrectLocationHeader()
        {
            _response.Headers.Location.Should().NotBeNull();
            _response.Headers.Location.AbsolutePath.Should().EndWith($"libraries/{_libraryId}/categories/{_category.Id}");
            return this;
        }

        public CategoryAssert ShouldHaveBooksLink()
        {
            _category.Link("books")
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/books")
                  .ShouldHaveQueryParameter("categoryid", _category.Id);

            return this;
        }

        public CategoryAssert ShouldHaveDeleteLink()
        {
            _category.DeleteLink()
                  .ShouldBeDelete()
                  .EndingWith($"libraries/{_libraryId}/categories/{_category.Id}");
            return this;
        }

        public CategoryAssert ShouldNotHaveUpdateLink()
        {
            _category.UpdateLink().Should().BeNull();

            return this;
        }

        public CategoryAssert ShouldNotHaveDeleteLink()
        {
            _category.DeleteLink().Should().BeNull();
            return this;
        }

        public CategoryAssert WithReadOnlyLinks()
        {
            ShouldNotHaveDeleteLink();
            ShouldNotHaveUpdateLink();
            return this;
        }

        public CategoryAssert ShouldHaveUpdatedCategory()
        {
            var dbCat = _categoryRepository.GetCategoryById(_libraryId, _category.Id);
            dbCat.Should().NotBeNull();
            dbCat.Id.Should().Be(_category.Id);
            dbCat.Name.Should().Be(_category.Name);
            return this;
        }

        public CategoryAssert WithBookCount(int bookCount)
        {
            _category.BookCount.Should().Be(bookCount);
            return this;
        }

        public CategoryAssert ShouldHaveUpdateLink()
        {
            _category.UpdateLink()
                  .ShouldBePut()
                  .EndingWith($"libraries/{_libraryId}/categories/{_category.Id}");

            return this;
        }

        public CategoryAssert ShouldHaveSelfLink(string url)
        {
            _category.SelfLink()
                  .ShouldBeGet()
                  .EndingWith(url);

            return this;
        }

        public CategoryAssert ShouldHaveSelfLink()
        {
            _category.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/categories/{_category.Id}");

            return this;
        }

        public CategoryAssert ShouldHaveCreatedCategory()
        {
            var cat = _categoryRepository.GetCategoryById(_libraryId, _category.Id);
            cat.Should().NotBeNull();
            return this;
        }

        public CategoryAssert ShouldBeSameAs(CategoryDto dto)
        {
            _category.Id.Should().Be(dto.Id);
            _category.Name.Should().Be(dto.Name);
            return this;
        }

        public CategoryAssert ShouldBeSameAs(CategoryView expected, bool matchLinks = false)
        {
            _category.Id.Should().Be(expected.Id);
            _category.Name.Should().Be(expected.Name);
            if (matchLinks)
            {
                _category.Links.Should().BeEquivalentTo(expected.Links);
            }
            return this;
        }
    }
}
