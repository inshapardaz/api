using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Library.Books.Files;
using Inshapardaz.Functions.Library.Files;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;

namespace Inshapardaz.Functions.Converters
{
    public static class FileConverter
    {
        public static FileView Render(this File source, ClaimsPrincipal user)
        {
            var result = source.Map();
            var links = new List<LinkView>
            {
                GetFileById.Link(source.Id)
            };

            if (user.IsWriter())
            {
                links.Add(DeleteFile.Link(source.Id, RelTypes.Delete));
            }

            result.Links = links;

            return result;
        }

        public static BookFilesView Render(this IEnumerable<File> source, ClaimsPrincipal user)
        {
            var result = new BookFilesView();
            result.Items = source.Select(f => f.Render(user));

            return result;
        }
    }
}
