using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Api.Views.Library;

namespace Inshapardaz.Api.Mappings
{
    public static class CategoryMapper
    {
        public static CategoryView Map(this CategoryModel source)
            => new CategoryView
            {
                Id = source.Id,
                Name = source.Name,
                BookCount = source.BookCount
            };

        public static CategoryModel Map(this CategoryView source)
            => new CategoryModel
            {
                Id = source.Id,
                Name = source.Name
            };
    }
}