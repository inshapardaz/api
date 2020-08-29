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
using System.Net.Mime;

namespace Inshapardaz.Api.Converters
{
    public interface IRenderChapter
    {
        ChapterContentView Render(ChapterContentModel source, int libraryId);

        ChapterView Render(ChapterModel source, int libraryId, int bookId);

        ListView<ChapterView> Render(IEnumerable<ChapterModel> source, int libraryId, int bookId);
    }

    public class ChapterRenderer : IRenderChapter
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public ChapterRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }

        public ListView<ChapterView> Render(IEnumerable<ChapterModel> source, int libraryId, int bookId)
        {
            var items = source.Select(c => Render(c, libraryId, bookId)).ToList();
            var view = new ListView<ChapterView> { Data = items };

            view.Links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(ChapterController.GetChaptersByBook),
                Method = HttpMethod.Get,
                Rel = RelTypes.Self,
                Parameters = new { libraryId = libraryId, bookId = bookId }
            }));

            if (_userHelper.IsWriter)
            {
                view.Links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ChapterController.CreateChapter),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.Create,
                    Parameters = new { libraryId = libraryId, bookId = bookId }
                }));
            }

            return view;
        }

        public ChapterView Render(ChapterModel source, int libraryId, int bookId)
        {
            var result = source.Map();
            var links = new List<LinkView>
            {
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(ChapterController.GetChapterById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Self,
                    Parameters = new { libraryId = libraryId, bookId = bookId, chapterId = source.Id }
                }),
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookController.GetBookById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Book,
                    Parameters = new { libraryId = libraryId, bookId = bookId }
                })
            };

            if (_userHelper.IsWriter)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ChapterController.UpdateChapter),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.Update,
                    Parameters = new { libraryId = libraryId, bookId = bookId, chapterId = source.Id }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ChapterController.DeleteChapter),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Delete,
                    Parameters = new { libraryId = libraryId, bookId = bookId, chapterId = source.Id }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ChapterController.CreateChapterContent),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.AddContent,
                    Parameters = new { libraryId = libraryId, bookId = bookId, chapterId = source.Id }
                }));
            }

            if (_userHelper.IsAuthenticated)
            {
                var contents = new List<ChapterContentView>();
                foreach (var content in source.Contents)
                {
                    contents.Add(Render(content, libraryId));
                }

                result.Contents = contents;
            }

            result.Links = links;
            return result;
        }

        public ChapterContentView Render(ChapterContentModel source, int libraryId)
        {
            var result = source.Map();

            var links = new List<LinkView>
            {
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(ChapterController.GetChapterContent),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Self,
                    MimeType = source.MimeType,
                    Language = source.Language,
                    Parameters = new { libraryId = libraryId, bookId = source.BookId, chapterId = source.ChapterId }
                }),
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(ChapterController.GetChapterById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Chapter,
                    Parameters = new { libraryId = libraryId, bookId = source.BookId, chapterId = source.ChapterId }
                }),
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookController.GetBookById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Book,
                    Parameters = new { libraryId = libraryId, bookId = source.BookId }
                })
        };

            if (!string.IsNullOrWhiteSpace(source.ContentUrl))
            {
                links.Add(new LinkView { Href = source.ContentUrl, Method = "GET", Rel = RelTypes.Download, Accept = MimeTypes.Jpg });
            }

            if (_userHelper.IsWriter)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ChapterController.UpdateChapterContent),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.Update,
                    MimeType = source.MimeType,
                    Language = source.Language,
                    Parameters = new { libraryId = libraryId, bookId = source.BookId, chapterId = source.ChapterId }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ChapterController.DeleteChapterContent),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Delete,
                    MimeType = source.MimeType,
                    Language = source.Language,
                    Parameters = new { libraryId = libraryId, bookId = source.BookId, chapterId = source.ChapterId }
                }));
            }

            result.Links = links;
            return result;
        }
    }
}
