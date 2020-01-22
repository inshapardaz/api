using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Functions.Views.Library;

namespace Inshapardaz.Functions.Mappings
{
    public static class SeriesMapper
    {
        public static SeriesView Map(this SeriesModel source)
            => new SeriesView
            {
                Id = source.Id,
                Name = source.Name,
                Description = source.Description,
                BookCount = source.BookCount
            };

        public static SeriesModel Map(this SeriesView source)
            => new SeriesModel
            {
                Id = source.Id,
                Name = source.Name,
                Description = source.Description
            };
    }
}
