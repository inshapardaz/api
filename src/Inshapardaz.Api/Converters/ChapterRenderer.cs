using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Library;

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

            if (_userHelper.IsWriter(libraryId) || _userHelper.IsLibraryAdmin(libraryId) || _userHelper.IsAdmin)
            {
                view.Links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ChapterController.UpdateChapterSequence),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.ChapterSequence,
                    Parameters = new { libraryId = libraryId, bookId = bookId }
                }));

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
                    Parameters = new { libraryId = libraryId, bookId = bookId, chapterNumber = source.ChapterNumber }
                }),
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookController.GetBookById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Book,
                    Parameters = new { libraryId = libraryId, bookId = bookId }
                })
            };

            if (source.PreviousChapter != null)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ChapterController.GetChapterById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Previous,
                    Parameters = new { libraryId = libraryId, bookId = source.PreviousChapter.BookId, chapterNumber = source.PreviousChapter.ChapterNumber }
                }));
            }

            if (source.NextChapter != null)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ChapterController.GetChapterById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Next,
                    Parameters = new { libraryId = libraryId, bookId = source.NextChapter.BookId, chapterNumber = source.NextChapter.ChapterNumber }
                }));
            }

            if (_userHelper.IsWriter(libraryId) || _userHelper.IsAdmin || _userHelper.IsLibraryAdmin(libraryId))
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ChapterController.UpdateChapter),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.Update,
                    Parameters = new { libraryId = libraryId, bookId = bookId, chapterNumber = source.ChapterNumber }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ChapterController.DeleteChapter),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Delete,
                    Parameters = new { libraryId = libraryId, bookId = bookId, chapterNumber = source.ChapterNumber }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ChapterController.CreateChapterContent),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.AddContent,
                    Parameters = new { libraryId = libraryId, bookId = bookId, chapterNumber = source.ChapterNumber }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ChapterController.AssignChapterToUser),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.Assign,
                    Parameters = new { libraryId = libraryId, bookId = source.BookId, chapterNumber = source.ChapterNumber }
                }));
            }

            if (_userHelper.IsAuthenticated)
            {
                var contents = new List<ChapterContentView>();
                foreach (var content in source.Contents)
                {
                    contents.Add(Render(content, libraryId, false));
                }

                result.Contents = contents;
            }

            result.Links = links;
            return result;
        }

        public ChapterContentView Render(ChapterContentModel source, int libraryId)
        {
            return Render(source, libraryId, true);
        }

        private ChapterContentView Render(ChapterContentModel source, int libraryId, bool addText = true)
        {
            var result = source.Map();

            if (!addText)
            {
                result.Text = null;
            }

            var links = new List<LinkView>
            {
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(ChapterController.GetChapterContent),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Self,
                    Language = source.Language,
                    Parameters = new { libraryId = libraryId, bookId = source.BookId, chapterNumber = source.ChapterNumber }
                }),
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(ChapterController.GetChapterById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Chapter,
                    Parameters = new { libraryId = libraryId, bookId = source.BookId, chapterNumber = source.ChapterNumber }
                }),
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(BookController.GetBookById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Book,
                    Parameters = new { libraryId = libraryId, bookId = source.BookId }
                })
        };

            links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(ChapterController.GetChapterContent),
                Method = HttpMethod.Get,
                Rel = RelTypes.Content,
                Language = source.Language,
                Parameters = new { libraryId = libraryId, bookId = source.BookId, chapterNumber = source.ChapterNumber }
            }));

            if (_userHelper.IsWriter(libraryId) || _userHelper.IsAdmin || _userHelper.IsLibraryAdmin(libraryId))
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ChapterController.UpdateChapterContent),

                    Method = HttpMethod.Put,
                    Rel = RelTypes.Update,
                    Language = source.Language,
                    Parameters = new
                    {
                        libraryId = libraryId,
                        bookId = source.BookId,
                        chapterNumber = source.ChapterNumber
                    }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ChapterController.DeleteChapterContent),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Delete,
                    Language = source.Language,
                    Parameters = new { libraryId = libraryId, bookId = source.BookId, chapterNumber = source.ChapterNumber }
                }));
            }

            result.Links = links;
            return result;
        }
    }
}
