using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Library.Authors;
using Inshapardaz.Functions.Library.Books;
using Inshapardaz.Functions.Library.Categories;
using Inshapardaz.Functions.Library.Periodicals;
using Inshapardaz.Functions.Library.Series;
using Inshapardaz.Functions.Views;
using System.Collections.Generic;
using System.Security.Claims;

namespace Inshapardaz.Functions.Converters
{
    public static class LibraryRenderer
    {
        public static LibraryView Render(this LibraryModel model, ClaimsPrincipal user)
        {
            var links = new List<LinkView>
            {
                GetLibrary.Link(model.Id),
                GetAuthors.Link(model.Id, RelTypes.Authors),
                GetCategories.Link(model.Id, RelTypes.Categories),
                GetSeries.Link(model.Id, RelTypes.Series),
                GetBooks.Link(model.Id, RelTypes.Books)
            };

            if (model.SupportsPeriodicals)
            {
                links.Add(GetPeriodicals.Link(model.Id, RelTypes.Periodicals));
            }

            if (user.IsAuthenticated())
            {
                links.Add(GetRecentReadBooks.Link(model.Id, RelTypes.Recents));
            }

            if (user.IsWriter())
            {
                links.Add(AddAuthor.Link(model.Id, RelTypes.CreateAuthor));
                links.Add(AddBook.Link(model.Id, RelTypes.CreateBook));
                links.Add(AddSeries.Link(model.Id, RelTypes.CreateSeries));
            }

            if (user.IsAdministrator())
            {
                links.Add(AddCategory.Link(model.Id, RelTypes.CreateCategory));
            }

            return new LibraryView { Links = links };
        }
    }
}
