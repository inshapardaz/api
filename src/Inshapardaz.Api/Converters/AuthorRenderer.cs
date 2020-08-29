using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using System.Collections.Generic;
using System.Linq;

namespace Inshapardaz.Api.Converters
{
    public interface IRenderAuthor
    {
        PageView<AuthorView> Render(PageRendererArgs<AuthorModel> source, int libraryId);

        AuthorView Render(AuthorModel source, int libraryId);
    }

    public class AuthorRenderer : IRenderAuthor
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public AuthorRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }

        public PageView<AuthorView> Render(PageRendererArgs<AuthorModel> source, int libraryId)
        {
            var page = new PageView<AuthorView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data?.Select(x => Render(x, libraryId))
            };

            var links = new List<LinkView>
            {
                _linkRenderer.Render(new Link {
                    ActionName = nameof(BookController.GetBooks),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Self,
                    Parameters = new { libraryId = libraryId },
                    QueryString = new Dictionary<string, string>()
                    {
                        { "pageNumber" , page.CurrentPageIndex.ToString() },
                        { "pageSize", page.PageSize.ToString() },
                        { "query", source.RouteArguments.Query }
                    }
                })
            };

            if (_userHelper.IsWriter)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(AuthorController.CreateAuthor),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.Create,
                    Parameters = new { libraryId = libraryId }
                }));
            }

            if (page.CurrentPageIndex < page.PageCount)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookController.GetBooks),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Next,
                    Parameters = new { libraryId = libraryId },
                    QueryString = new Dictionary<string, string>()
                    {
                        { "pageNumber" , (page.CurrentPageIndex + 1).ToString() },
                        { "pageSize", page.PageSize.ToString() },
                        { "query", source.RouteArguments.Query }
                    }
                }));
            }

            if (page.PageCount > 1 && page.CurrentPageIndex > 1 && page.CurrentPageIndex <= page.PageCount)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookController.GetBooks),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Previous,
                    Parameters = new { libraryId = libraryId },
                    QueryString = new Dictionary<string, string>()
                    {
                        { "pageNumber" , (page.CurrentPageIndex - 1).ToString() },
                        { "pageSize", page.PageSize.ToString() },
                        { "query", source.RouteArguments.Query }
                    }
                }));
            }

            page.Links = links;
            return page;
        }

        public AuthorView Render(AuthorModel source, int libraryId)
        {
            var result = source.Map();

            var links = new List<LinkView>
            {
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(AuthorController.GetAuthorById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Self,
                    Parameters = new { libraryId = libraryId, authorId = source.Id }
                }),
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookController.GetBooks),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Books,
                    Parameters = new { libraryId = libraryId, authorId = source.Id },
                    QueryString = new Dictionary<string, string>
                    {
                        { "authorid", source.Id.ToString() }
                    }
                })
            };

            if (!string.IsNullOrWhiteSpace(source.ImageUrl))

            {
                links.Add(new LinkView
                {
                    Href = source.ImageUrl,
                    Method = "GET",
                    Rel = RelTypes.Image,
                    Accept = MimeTypes.Jpg
                });
            }
            else if (source.ImageId.HasValue)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(FileController.GetFile),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Image,
                    Parameters = new { fileId = source.ImageId.Value }
                }));
            }

            if (_userHelper.IsWriter)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(AuthorController.UpdateAuthor),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.Update,
                    Parameters = new { libraryId = libraryId, authorId = source.Id }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(AuthorController.DeleteAuthor),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Delete,
                    Parameters = new { libraryId = libraryId, authorId = source.Id }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(AuthorController.UpdateAuthorImage),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.ImageUpload,
                    Parameters = new { libraryId = libraryId, authorId = source.Id }
                }));
            }

            result.Links = links;
            return result;
        }
    }
}
