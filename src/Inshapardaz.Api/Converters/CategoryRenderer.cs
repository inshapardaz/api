using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Library;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using System.Collections.Generic;
using System.Linq;

namespace Inshapardaz.Api.Converters
{
    public interface IRenderCategory
    {
        CategoryView Render(CategoryModel category, int libraryId);

        ListView<CategoryView> Render(IEnumerable<CategoryModel> categories, int libraryId);
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

        public ListView<CategoryView> Render(IEnumerable<CategoryModel> categories, int libraryId)
        {
            var items = categories.Select(g => Render(g, libraryId));
            var view = new ListView<CategoryView> { Data = items };
            view.Links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(CategoryController.GetCategories),
                Method = HttpMethod.Get,
                Rel = RelTypes.Self,
                Parameters = new { libraryId = libraryId },
            }));

            if (_userHelper.IsAdmin || _userHelper.IsLibraryAdmin)
            {
                view.Links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(CategoryController.CreateCategory),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.Create,
                    Parameters = new { libraryId = libraryId },
                }));
            }

            return view;
        }

        public CategoryView Render(CategoryModel category, int libraryId)
        {
            var view = category.Map();

            view.Links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(CategoryController.GetCategoryById),
                Method = HttpMethod.Get,
                Rel = RelTypes.Self,
                Parameters = new { libraryId = libraryId, categoryId = category.Id }
            }));

            view.Links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(BookController.GetBooks),
                Method = HttpMethod.Get,
                Rel = RelTypes.Books,
                Parameters = new { libraryId = libraryId },
                QueryString = new Dictionary<string, string>
                {
                    { "categoryid", category.Id.ToString() }
                }
            }));

            if (_userHelper.IsAdmin || _userHelper.IsLibraryAdmin)
            {
                view.Links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(CategoryController.UpdateCategory),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.Update,
                    Parameters = new { libraryId = libraryId, categoryId = category.Id }
                }));

                view.Links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(CategoryController.DeleteCategory),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Delete,
                    Parameters = new { libraryId = libraryId, categoryId = category.Id }
                }));
            }

            return view;
        }
    }
}
