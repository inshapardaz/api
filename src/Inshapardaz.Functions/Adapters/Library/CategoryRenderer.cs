using System.Security.Claims;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Library.Books;
using Inshapardaz.Functions.Library.Categories;
using Inshapardaz.Functions.Views;
using Inshapardaz.Functions.Views.Library;

namespace Inshapardaz.Functions.Adapters.Library
{
    public interface IRenderCategory
    {
        CategoryView Render(ClaimsPrincipal principal, Category category);
    }

    public class CategoryRenderer : IRenderCategory
    {
        public CategoryView Render(ClaimsPrincipal principal, Category category)
        {
            var view = category.Map<Category, CategoryView>();

            view.Links.Add(GetCategoryById.Self(category.Id, RelTypes.Self));
            view.Links.Add(GetBooksByCategory.Self(category.Id, RelTypes.Books));

            if (principal.IsWriter())
            {
                view.Links.Add(UpdateCategory.Link(category.Id, RelTypes.Update));
                view.Links.Add(DeleteCategory.Link(category.Id, RelTypes.Delete));
            }

            return view;
        }
    }
}
