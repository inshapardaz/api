using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Api.Converters;

public interface IRenderCategory
{
    CategoryView Render(CategoryModel category, int libraryId);

    ListView<CategoryView> Render(IEnumerable<CategoryModel> categories, int libraryId);
}

public class CategoryRenderer(IRenderLink linkRenderer, IUserHelper userHelper) : IRenderCategory
{
    public ListView<CategoryView> Render(IEnumerable<CategoryModel> categories, int libraryId)
    {
        var items = categories.Select(g => Render(g, libraryId));
        var view = new ListView<CategoryView> { Data = items };
        view.Links.Add(linkRenderer.Render(new Link
        {
            ActionName = nameof(CategoryController.GetCategories),
            Method = HttpMethod.Get,
            Rel = RelTypes.Self,
            Parameters = new { libraryId = libraryId },
        }));

        view.Links.Add(linkRenderer.Render(new Link
        {
            ActionName = nameof(CategoryController.GetCategoryTree),
            Method = HttpMethod.Get,
            Rel = RelTypes.Tree,
            Parameters = new { libraryId = libraryId },
        }));

        if (userHelper.IsAdmin || userHelper.IsLibraryAdmin(libraryId))
        {
            view.Links.Add(linkRenderer.Render(new Link
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

        view.Links.Add(linkRenderer.Render(new Link
        {
            ActionName = nameof(CategoryController.GetCategoryById),
            Method = HttpMethod.Get,
            Rel = RelTypes.Self,
            Parameters = new { libraryId = libraryId, categoryId = category.Id }
        }));

        view.Links.Add(linkRenderer.Render(new Link
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

        view.Links.Add(linkRenderer.Render(new Link
        {
            ActionName = nameof(CategoryController.GetChildCategories),
            Method = HttpMethod.Get,
            Rel = RelTypes.Children,
            Parameters = new { libraryId = libraryId, categoryId = category.Id }
        }));

        if (category.ParentCategoryId.HasValue)
        {
            view.Links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(CategoryController.GetCategoryById),
                Method = HttpMethod.Get,
                Rel = RelTypes.Parent,
                Parameters = new { libraryId = libraryId, categoryId = category.ParentCategoryId.Value }
            }));
        }

        if (userHelper.IsAdmin || userHelper.IsLibraryAdmin(libraryId))
        {
            view.Links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(CategoryController.UpdateCategory),
                Method = HttpMethod.Put,
                Rel = RelTypes.Update,
                Parameters = new { libraryId = libraryId, categoryId = category.Id }
            }));

            view.Links.Add(linkRenderer.Render(new Link
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
