using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Functions.Views.Library;

namespace Inshapardaz.Functions.Mappings
{
    public static class AuthorMapper
    {
        public static AuthorView Map(this AuthorModel source)
            => new AuthorView
            {
                Id = source.Id,
                Name = source.Name,
                BookCount = source.BookCount
            };

        public static AuthorModel Map(this AuthorView source)
            => new AuthorModel
            {
                Id = source.Id,
                Name = source.Name
            };
    }
}
