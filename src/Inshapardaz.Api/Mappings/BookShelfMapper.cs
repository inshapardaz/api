
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Api.Views.Library;

namespace Inshapardaz.Api.Mappings
{
    public static class BookShelfMapper
    {
        public static BookShelfView Map(this BookShelfModel source)
            => source == null ? null : new BookShelfView
            {
                Id = source.Id,
                Name = source.Name,
                Description = source.Description,
                BookCount = source.BookCount,
                IsPublic = source.IsPublic
            };

        public static BookShelfModel Map(this BookShelfView source)
            => source == null ? null : new BookShelfModel
            {
                Id = source.Id,
                Name = source.Name,
                Description = source.Description,
                BookCount = source.BookCount,
                IsPublic = source.IsPublic
            };
    }
}
