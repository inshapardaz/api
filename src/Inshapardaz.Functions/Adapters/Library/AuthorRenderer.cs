using System.Collections.Generic;
using System.Security.Claims;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Library.Authors;
using Inshapardaz.Functions.Library.Books;
using Inshapardaz.Functions.Library.Files;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;

namespace Inshapardaz.Functions.Adapters.Library
{
    public interface IRenderAuthor
    {
        AuthorView Render(ClaimsPrincipal principal, Author source);
    }

    public class AuthorRenderer : IRenderAuthor
    {
        public AuthorView Render(ClaimsPrincipal principal, Author source)
        {
            var result = source.Map<Author, AuthorView>();

            var links = new List<LinkView>
            {
                GetAuthorById.Link(source.Id, RelTypes.Self),
                GetBooksByAuthor.Link(source.Id, RelTypes.Books)
            };

            if (source.ImageId > 0)
            {
                links.Add(GetFileById.Link(source.ImageId, RelTypes.Image));
            }

            if (principal.IsWriter())
            {
                links.Add(UpdateAuthor.Link(source.Id, RelTypes.Update));
                links.Add(DeleteAuthor.Link(source.Id, RelTypes.Delete));
                links.Add(UpdateAuthorImage.Link(source.Id, RelTypes.ImageUpload));
            }

            result.Links = links;
            return result;
        }
    }
}
