using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;

namespace Inshapardaz.Api.Renderers.Library
{
    public interface IRenderCategory
    {
        CategoryView RenderResult(Category category);
    }

    public class CategoryRenderer : IRenderCategory
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public CategoryRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }
        public CategoryView RenderResult(Category category)
        {
            var view = category.Map<Category, CategoryView>();

            view.Links.Add(_linkRenderer.Render("GetCategoryById", RelTypes.Self, new { id = category.Id }));
            view.Links.Add(_linkRenderer.Render("GetBooksByCategory", RelTypes.Books, new { id = category.Id }));

            if (_userHelper.IsAdmin)
            {
                view.Links.Add(_linkRenderer.Render("UpdateCategory", RelTypes.Update, new { id = category.Id }));
                view.Links.Add(_linkRenderer.Render("DeleteCategory", RelTypes.Delete, new { id = category.Id }));
            }

            return view;
        }
    }
}
