using Inshapardaz.Api.Views.Library;

namespace Inshapardaz.Api.Tests.Framework.Dto
{
    public class CategoryDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int LibraryId { get; set; }

        public int? ParentCategoryId { get; set; }

        public CategoryView ToView()
        {
            return new CategoryView
            {
                Id =  Id,
                Name = Name,
                ParentCategoryId = ParentCategoryId,
            };
        }
    }
}
