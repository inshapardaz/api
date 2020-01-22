using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Library.Books;
using Inshapardaz.Functions.Library.Categories;
using Inshapardaz.Functions.Mappings;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;

namespace Inshapardaz.Functions.Converters
{
    public static class CategoryRenderer
    {
        public static ListView<CategoryView> Render(this IEnumerable<CategoryModel> categories, ClaimsPrincipal user)
        {
            var items = categories.Select(g => g.Render(user));
            var view = new ListView<CategoryView> { Items = items };
            view.Links.Add(GetCategories.Link());

            if (user.IsAdministrator())
            {
                view.Links.Add(AddCategory.Link(RelTypes.Create));
            }

            return view;
        }

        public static CategoryView Render(this CategoryModel category, ClaimsPrincipal principal)
        {
            var view = category.Map();

            view.Links.Add(GetCategoryById.Link(category.Id));
            view.Links.Add(GetBooksByCategory.Self(category.Id, RelTypes.Books));

            if (principal.IsAdministrator())
            {
                view.Links.Add(UpdateCategory.Link(category.Id, RelTypes.Update));
                view.Links.Add(DeleteCategory.Link(category.Id, RelTypes.Delete));
            }

            return view;
        }
    }
}
