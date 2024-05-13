using Inshapardaz.Domain.Models;
using Inshapardaz.Api.Views;

namespace Inshapardaz.Api.Mappings
{
    public static class FileMapper
    {
        public static FileView Map(this FileModel source)
            => new FileView
            {
                Id = source.Id,
                MimeType = source.MimeType,
                FileName = source.FileName,
                DateCreated = source.DateCreated,
            };

        public static FileModel Map(this FileView source)
            => new FileModel
            {
                Id = source.Id,
                MimeType = source.MimeType,
                FileName = source.FileName,
                DateCreated = source.DateCreated
            };
    }
}