using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Repositories;

namespace Inshapardaz.Api.Converters
{
    public interface IRenderSeries
    {
        PageView<SeriesView> Render(PageRendererArgs<SeriesModel> source, int libraryId);

        SeriesView Render(SeriesModel series, int libraryId);
    }

    public class SeriesRenderer : IRenderSeries
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;
        private readonly IFileStorage _fileStorage;

        public SeriesRenderer(IRenderLink linkRenderer, IUserHelper userHelper, IFileStorage fileStorage)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
            _fileStorage = fileStorage;
        }

        public PageView<SeriesView> Render(PageRendererArgs<SeriesModel> source, int libraryId)
        {
            var page = new PageView<SeriesView>(source.Page.TotalCount, source.Page.PageSize, source.Page.PageNumber)
            {
                Data = source.Page.Data?.Select(x => Render(x, libraryId))
            };

            var links = new List<LinkView>
            {
                _linkRenderer.Render(new Link {
                    ActionName = nameof(SeriesController.GetSeries),
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

            if (_userHelper.IsWriter(libraryId) || _userHelper.IsLibraryAdmin(libraryId) || _userHelper.IsAdmin)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(SeriesController.CreateSeries),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.Create,
                    Parameters = new { libraryId = libraryId }
                }));
            }

            if (page.CurrentPageIndex < page.PageCount)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(SeriesController.GetSeries),
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
                    ActionName = nameof(SeriesController.GetSeries),
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

        public SeriesView Render(SeriesModel series, int libraryId)
        {
            var view = series.Map();

            view.Links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(SeriesController.GetSeriesById),
                Method = HttpMethod.Get,
                Rel = RelTypes.Self,
                Parameters = new { libraryId = libraryId, seriesId = series.Id }
            }));

            view.Links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(BookController.GetBooks),
                Method = HttpMethod.Get,
                Rel = RelTypes.Books,
                Parameters = new { libraryId = libraryId },
                QueryString = new Dictionary<string, string>
                {
                    { "seriesid", series.Id.ToString() }
                }
            }));

            if (!string.IsNullOrWhiteSpace(series.ImageUrl) && _fileStorage.SupportsPublicLink)
            {
                view.Links.Add(new LinkView { 
                    Href = _fileStorage.GetPublicUrl(series.ImageUrl), 
                    Method = "GET", 
                    Rel = RelTypes.Image, 
                    Accept = MimeTypes.Jpg 
                });
            }
            else if (series.ImageId.HasValue)
            {
                view.Links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(FileController.GetLibraryFile),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Image,
                    Parameters = new { libraryId = libraryId, fileId = series.ImageId.Value }
                }));
            }

            if (_userHelper.IsWriter(libraryId) || _userHelper.IsLibraryAdmin(libraryId) || _userHelper.IsAdmin)
            {
                view.Links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(SeriesController.UpdateSeries),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.Update,
                    Parameters = new { libraryId = libraryId, seriesId = series.Id }
                }));

                view.Links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(SeriesController.DeleteSeries),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Delete,
                    Parameters = new { libraryId = libraryId, seriesId = series.Id }
                }));

                view.Links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(SeriesController.UpdateSeriesImage),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.ImageUpload,
                    Parameters = new { libraryId = libraryId, seriesId = series.Id }
                }));
            }

            return view;
        }
    }
}
