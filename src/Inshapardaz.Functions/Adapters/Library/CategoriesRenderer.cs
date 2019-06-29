using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Functions.Library.Categories;
using Inshapardaz.Functions.View.Library;
using Inshapardaz.Functions.Views;

namespace Inshapardaz.Functions.Adapters.Library
{
    public interface IRenderCategories
    {
        ListView<CategoryView> Render(IEnumerable<Category> categories);
    }

    public class CategoriesRenderer : IRenderCategories
    {
        private readonly IRenderCategory _categoryRenderer;

        public CategoriesRenderer(IRenderCategory categoryRenderer)
        {
            _categoryRenderer = categoryRenderer;
        }

        public ListView<CategoryView> Render(IEnumerable<Category> categories)
        {
            var items = categories.Select(g => _categoryRenderer.RenderResult(g));
            var view = new ListView<CategoryView> { Items = items };
            view.Links.Add(GetCategories.Self(RelTypes.Self));

            /*if (_userHelper.IsAdmin)
            {
                view.Links.Add(_linkRenderer.Render("CreateCategory", RelTypes.Create));
            }*/

            return view;
        }
    }
}
