using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Functions.Library.Books;
using Inshapardaz.Functions.Library.Categories;
using Inshapardaz.Functions.View.Library;
using Inshapardaz.Functions.Views;

namespace Inshapardaz.Functions.Adapters.Library
{
    public interface IRenderCategory
    {
        CategoryView RenderResult(Category category);
    }

    public class CategoryRenderer : IRenderCategory
    {
        public CategoryView RenderResult(Category category)
        {
            var view = category.Map<Category, CategoryView>();

            view.Links.Add(GetCategoryById.Self(category.Id, RelTypes.Self));
            view.Links.Add(GetBooksByCategory.Self(category.Id, RelTypes.Books));

            /*if (_userHelper.IsAdmin)
            {
                view.Links.Add(_linkRenderer.Render("UpdateCategory", RelTypes.Update, new { id = category.Id }));
                view.Links.Add(_linkRenderer.Render("DeleteCategory", RelTypes.Delete, new { id = category.Id }));
            }*/

            return view;
        }
    }
}
