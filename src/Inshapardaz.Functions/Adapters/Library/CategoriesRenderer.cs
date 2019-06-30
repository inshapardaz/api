using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Library.Categories;
using Inshapardaz.Functions.View.Library;
using Inshapardaz.Functions.Views;

namespace Inshapardaz.Functions.Adapters.Library
{
    public interface IRenderCategories
    {
        ListView<CategoryView> Render(ClaimsPrincipal user, IEnumerable<Category> categories);
    }

    public class CategoriesRenderer : IRenderCategories
    {
        private readonly IRenderCategory _categoryRenderer;

        public CategoriesRenderer(IRenderCategory categoryRenderer)
        {
            _categoryRenderer = categoryRenderer;
        }

        public ListView<CategoryView> Render(ClaimsPrincipal user, IEnumerable<Category> categories)
        {
            var items = categories.Select(g => _categoryRenderer.RenderResult(g));
            var view = new ListView<CategoryView> { Items = items };
            view.Links.Add(GetCategories.Link(RelTypes.Self));

            if (user.IsAdministrator())
            {
                view.Links.Add(AddCategory.Link(RelTypes.Create));
            }

            return view;
        }
    }
}
