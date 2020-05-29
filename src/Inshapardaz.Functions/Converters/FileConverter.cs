using Inshapardaz.Domain.Models;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Library.Files;
using Inshapardaz.Functions.Mappings;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Inshapardaz.Functions.Converters
{
    public static class FileConverter
    {
        public static FileView Render(this FileModel source, ClaimsPrincipal user)
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

        public static BookContentView Render(this IEnumerable<FileModel> source, ClaimsPrincipal user)
        {
            var result = new BookContentView();
            //result.Items = source.Select(f => f.Render(user));

            return result;
        }
    }
}
