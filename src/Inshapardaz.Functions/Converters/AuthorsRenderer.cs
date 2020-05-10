using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Library.Authors;
using Inshapardaz.Functions.Library.Books;
using Inshapardaz.Functions.Library.Files;
using Inshapardaz.Functions.Mappings;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Inshapardaz.Functions.Converters
{
    public static class AuthorsRenderer
    {
        public static PageView<AuthorView> Render(this PageRendererArgs<AuthorModel> source, int libraryId, ClaimsPrincipal principal)
        {
            var page = new PageView<AuthorView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data?.Select(x => x.Render(libraryId, principal))
            };

            var links = new List<LinkView>
            {
                source.LinkFuncWithParameter(libraryId, page.CurrentPageIndex, page.PageSize, source.RouteArguments.Query, RelTypes.Self)
            };

            if (principal.IsWriter())
            {
                links.Add(AddAuthor.Link(libraryId, RelTypes.Create));
            }

            if (page.CurrentPageIndex < page.PageCount)
            {
                links.Add(source.LinkFuncWithParameter(libraryId, page.CurrentPageIndex + 1, page.PageSize, source.RouteArguments.Query, RelTypes.Next));
            }

            if (page.PageCount > 1 && page.CurrentPageIndex > 1 && page.CurrentPageIndex <= page.PageCount)
            {
                links.Add(source.LinkFuncWithParameter(libraryId, page.CurrentPageIndex - 1, page.PageSize, source.RouteArguments.Query, RelTypes.Previous));
            }

            page.Links = links;
            return page;
        }

        public static AuthorView Render(this AuthorModel source, int libraryId, ClaimsPrincipal principal)
        {
            var result = source.Map();

            var links = new List<LinkView>
            {
                GetAuthorById.Link(libraryId, source.Id, RelTypes.Self),
                GetBooksByAuthor.Link(libraryId, source.Id, RelTypes.Books)
            };

            if (!string.IsNullOrWhiteSpace(source.ImageUrl))
            {
                links.Add(new LinkView { Href = source.ImageUrl, Method = "GET", Rel = RelTypes.Image, Media = MimeTypes.Jpg });
            }
            else if (source.ImageId.HasValue)
            {
                links.Add(GetFileById.Link(source.ImageId.Value, RelTypes.Image));
            }

            if (principal.IsWriter())
            {
                links.Add(UpdateAuthor.Link(libraryId, source.Id, RelTypes.Update));
                links.Add(DeleteAuthor.Link(libraryId, source.Id, RelTypes.Delete));
                links.Add(UpdateAuthorImage.Link(libraryId, source.Id, RelTypes.ImageUpload));
            }

            result.Links = links;
            return result;
        }
    }
}
