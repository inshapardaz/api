using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;

namespace Inshapardaz.Api.Renderers.Library
{
    public interface IRenderCategories
    {
        ListView<CategoryView> RenderResult(IEnumerable<Category> categories);
    }

    public class CategoriesRenderer : IRenderCategories
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;
        private readonly IRenderCategory _categoryRenderer;

        public CategoriesRenderer(IRenderLink linkRenderer, IUserHelper userHelper, IRenderCategory categoryRenderer)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
            _categoryRenderer = categoryRenderer;
        }

        public ListView<CategoryView> RenderResult(IEnumerable<Category> categories)
        {
            var items = categories.Select(g => _categoryRenderer.RenderResult(g));
            var view = new ListView<CategoryView> { Items = items };
            view.Links.Add(_linkRenderer.Render("GetCategories", RelTypes.Self));

            if (_userHelper.IsAdmin)
            {
                view.Links.Add(_linkRenderer.Render("CreateCategory", RelTypes.Create));
            }

            return view;
        }
    }
}
