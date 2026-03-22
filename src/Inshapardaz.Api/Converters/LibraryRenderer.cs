using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;

namespace Inshapardaz.Api.Converters;

public interface IRenderLibrary
{
    PageView<LibraryView> Render(PageRendererArgs<LibraryModel> source);

    LibraryView Render(LibraryModel model);
}

public class LibraryRenderer(IRenderLink linkRenderer, IUserHelper userHelper, IFileStorage fileStorage)
    : IRenderLibrary
{
    public PageView<LibraryView> Render(PageRendererArgs<LibraryModel> source)
    {
        var page = new PageView<LibraryView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
        {
            Data = source.Page.Data?.Select(x => Render(x))
        };

        var queryString = new Dictionary<string, string>()
        {
            { "pageNumber" , page.CurrentPageIndex.ToString() },
            { "pageSize", page.PageSize.ToString() },
            { "query", source.RouteArguments.Query }
        };

        if (source.RouteArguments.AccountId.HasValue)
        {
            queryString.Add("accountId", source.RouteArguments.AccountId.Value.ToString());
        }

        var links = new List<LinkView>
        {
            linkRenderer.Render(new Link {
                ActionName = nameof(LibraryController.GetLibraries),
                Method = HttpMethod.Get,
                Rel = RelTypes.Self,
                Parameters = new object(),
                QueryString = queryString
            })
        };

        if (userHelper.IsAdmin)
        {
            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(LibraryController.CreateLibrary),
                Method = HttpMethod.Post,
                Rel = RelTypes.Create
            }));
        }

        if (page.CurrentPageIndex < page.PageCount)
        {
            var queryStringNext = new Dictionary<string, string>()
            {
                { "pageNumber" , (page.CurrentPageIndex + 1).ToString() },
                { "pageSize", page.PageSize.ToString() },
                { "query", source.RouteArguments.Query }
            };

            if (source.RouteArguments.AccountId.HasValue)
            {
                queryStringNext.Add("accountId", source.RouteArguments.AccountId.Value.ToString());
            }

            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(LibraryController.GetLibraries),
                Method = HttpMethod.Get,
                Rel = RelTypes.Next,
                Parameters = new object(),
                QueryString = queryStringNext
            }));
        }

        if (page.PageCount > 1 && page.CurrentPageIndex > 1 && page.CurrentPageIndex <= page.PageCount)
        {
            var queryStringPrev = new Dictionary<string, string>()
            {
                { "pageNumber" , (page.CurrentPageIndex - 1).ToString() },
                { "pageSize", page.PageSize.ToString() },
                { "query", source.RouteArguments.Query }
            };

            if (source.RouteArguments.AccountId.HasValue)
            {
                queryStringPrev.Add("accountId", source.RouteArguments.AccountId.Value.ToString());
            }

            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(LibraryController.GetLibraries),
                Method = HttpMethod.Get,
                Rel = RelTypes.Previous,
                QueryString = queryStringPrev
            }));
        }

        page.Links = links;
        return page;
    }

    public LibraryView Render(LibraryModel model)
    {
        var links = new List<LinkView>
        {
            linkRenderer.Render(new Link
            {
                ActionName = nameof(LibraryController.GetLibraryById),
                Method = HttpMethod.Get,
                Rel = RelTypes.Self,
                Parameters = new { libraryId = model.Id }
            }),

            linkRenderer.Render(new Link
            {
                ActionName = nameof(AuthorController.GetAuthors),
                Method = HttpMethod.Get,
                Rel = RelTypes.Authors,
                Parameters = new { libraryId = model.Id }
            }),
            linkRenderer.Render(new Link
            {
                ActionName = nameof(CategoryController.GetCategories),
                Method = HttpMethod.Get,
                Rel = RelTypes.Categories,
                Parameters = new { libraryId = model.Id }
            }),
            linkRenderer.Render(new Link
            {
                ActionName = nameof(SeriesController.GetSeries),
                Method = HttpMethod.Get,
                Rel = RelTypes.Series,
                Parameters = new { libraryId = model.Id }
            }),
            linkRenderer.Render(new Link
            {
                ActionName = nameof(BookController.GetBooks),
                Method = HttpMethod.Get,
                Rel = RelTypes.Books,
                Parameters = new { libraryId = model.Id }
            })
        };

        if (model.SupportsPeriodicals)
        {
            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(PeriodicalController.GetPeriodicals),
                Method = HttpMethod.Get,
                Rel = RelTypes.Periodicals,
                Parameters = new { libraryId = model.Id }
            }));
        }

        if (!string.IsNullOrWhiteSpace(model.ImageUrl) && fileStorage.SupportsPublicLink)
        {
            links.Add(new LinkView
            {
                Href = fileStorage.GetPublicUrl(model.ImageUrl),
                Method = "GET",
                Rel = RelTypes.Image,
                Accept = MimeTypes.Jpg
            });
        }
        else if (model.ImageId.HasValue)
        {
            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(FileController.GetLibraryFile),
                Method = HttpMethod.Get,
                Rel = RelTypes.Image,
                Parameters = new { libraryId = model.Id, fileId = model.ImageId.Value }
            }));
        }


        if (userHelper.IsAuthenticated)
        {
            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(BookController.GetBooks),
                Method = HttpMethod.Get,
                Rel = RelTypes.Recents,
                Parameters = new { libraryId = model.Id },
                QueryString = new Dictionary<string, string> { { "read", bool.TrueString } }
            }));
        }

        if (userHelper.IsWriter(model.Id) || userHelper.IsAdmin || userHelper.IsLibraryAdmin(model.Id))
        {
            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(BookController.CreateBook),
                Method = HttpMethod.Post,
                Rel = RelTypes.CreateBook,
                Parameters = new { libraryId = model.Id }
            }));

            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(AuthorController.CreateAuthor),
                Method = HttpMethod.Post,
                Rel = RelTypes.CreateAuthor,
                Parameters = new { libraryId = model.Id }
            }));

            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(SeriesController.CreateSeries),
                Method = HttpMethod.Post,
                Rel = RelTypes.CreateSeries,
                Parameters = new { libraryId = model.Id }
            }));

            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(UserController.GetBookPagesByUser),
                Method = HttpMethod.Get,
                Rel = RelTypes.MyPages,
                Parameters = new { libraryId = model.Id }
            }));

            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(UserController.GetBooksByUser),
                Method = HttpMethod.Get,
                Rel = RelTypes.MyPublishing,
                Parameters = new { libraryId = model.Id }
            }));

            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(UserController.GetUserPublicationSummary),
                Method = HttpMethod.Get,
                Rel = RelTypes.MyPublishingSummary,
                Parameters = new { libraryId = model.Id }
            }));

            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(PeriodicalController.CreatePeriodical),
                Method = HttpMethod.Get,
                Rel = RelTypes.CreatePeriodical,
                Parameters = new { libraryId = model.Id }
            }));
        }

        if (userHelper.IsLibraryAdmin(model.Id) || userHelper.IsAdmin)
        {
            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(LibraryController.UpdateLibrary),
                Method = HttpMethod.Put,
                Rel = RelTypes.Update,
                Parameters = new { libraryId = model.Id }
            }));

            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(LibraryController.UpdateLibraryImage),
                Method = HttpMethod.Put,
                Rel = RelTypes.ImageUpload,
                Parameters = new { libraryId = model.Id }
            }));

            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(CategoryController.CreateCategory),
                Method = HttpMethod.Post,
                Rel = RelTypes.CreateCategory,
                Parameters = new { libraryId = model.Id }
            }));

            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(AccountsController.GetLibraryUsers),
                Method = HttpMethod.Get,
                Rel = RelTypes.Users,
                Parameters = new { libraryId = model.Id }
            }));

            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(AccountsController.InviteUser),
                Method = HttpMethod.Get,
                Rel = RelTypes.AddUser,
                Parameters = new { libraryId = model.Id }
            }));
        }

        if (userHelper.IsAdmin)
        {
            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(LibraryController.DeleteLibrary),
                Method = HttpMethod.Delete,
                Rel = RelTypes.Delete,
                Parameters = new { libraryId = model.Id }
            }));
        }

        return new LibraryView
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            OwnerEmail = userHelper.IsAdmin ? model.OwnerEmail : model.OwnerEmail.MaskEmail(),
            Language = model.Language,
            SupportsPeriodicals = model.SupportsPeriodicals,
            PrimaryColor = model.PrimaryColor,
            SecondaryColor = model.SecondaryColor,
            Public = model.Public,
            Links = links,
            DatabaseConnection = userHelper.IsAdmin ? model.DatabaseConnection : null,
            FileStoreType = userHelper.IsAdmin ? model.FileStoreType.ToDescription() : null,
            FileStoreSource = userHelper.IsAdmin ? model.FileStoreSource : null
        };
    }
}
