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
    public interface IRenderArticle
    {
        ArticleContentView Render(ArticleContentModel source, int libraryId);

        ArticleView Render(ArticleModel source, int libraryId, int periodicalId, int volumeNumber, int issueNumber);

        ListView<ArticleView> Render(IEnumerable<ArticleModel> source, int libraryId, int periodicalId, int volumeNumber, int issueNumber);
    }

    public class ArticleRenderer : IRenderArticle
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public ArticleRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }

        public ListView<ArticleView> Render(IEnumerable<ArticleModel> source, int libraryId, int periodicalId, int volumeNumber, int issueNumber)
        {
            var items = source.Select(c => Render(c, libraryId, periodicalId, volumeNumber, issueNumber)).ToList();
            var view = new ListView<ArticleView> { Data = items };

            view.Links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(ArticleController.GetArticlesByIssue),
                Method = HttpMethod.Get,
                Rel = RelTypes.Self,
                Parameters = new { libraryId = libraryId, periodicalId = periodicalId, volumeNumber = volumeNumber, issueNumber = issueNumber }
            }));

            if (_userHelper.IsWriter(libraryId))
            {
                view.Links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.CreateArticle),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.Create,
                    Parameters = new { libraryId = libraryId, periodicalId = periodicalId, volumeNumber = volumeNumber, issueNumber = issueNumber }
                }));
            }

            return view;
        }

        public ArticleView Render(ArticleModel source, int libraryId, int periodicalId, int volumeNumber, int issueNumber)
        {
            var result = source.Map();
            var links = new List<LinkView>
            {
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.GetArticleById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Self,
                    Parameters = new { libraryId = libraryId, periodicalId = periodicalId, volumeNumber = volumeNumber, issueNumber = issueNumber, articleId = source.Id }
                }),
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(PeriodicalController.GetPeriodicalById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Periodical,
                    Parameters = new { libraryId = libraryId, periodicalId = periodicalId }
                }),
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueController.GetIssueById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Issue,
                    Parameters = new { libraryId = libraryId, periodicalId = periodicalId, volumeNumber = volumeNumber, issueNumber = issueNumber }
                })
            };

            if (_userHelper.IsWriter(libraryId))
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.UpdateArticle),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.Update,
                    Parameters = new { libraryId = libraryId, periodicalId = periodicalId, volumeNumber = volumeNumber, issueNumber = issueNumber, articleId = source.Id }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.DeleteArticle),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Delete,
                    Parameters = new { libraryId = libraryId, periodicalId = periodicalId, volumeNumber = volumeNumber, issueNumber = issueNumber, articleId = source.Id }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.CreateArticleContent),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.AddContent,
                    Parameters = new { libraryId = libraryId, periodicalId = periodicalId, volumeNumber = volumeNumber, issueNumber = issueNumber, articleId = source.Id }
                }));
            }

            if (_userHelper.IsAuthenticated)
            {
                var contents = new List<ArticleContentView>();
                foreach (var content in source.Contents)
                {
                    contents.Add(Render(content, libraryId));
                }

                result.Contents = contents;
            }

            result.Links = links;
            return result;
        }

        public ArticleContentView Render(ArticleContentModel source, int libraryId)
        {
            var result = source.Map();

            var links = new List<LinkView>
            {
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.GetArticleContent),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Self,
                    MimeType = source.MimeType,
                    Language = source.Language,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId, issueNumber = source.IssueId, articleId = source.ArticleId }
                }),
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.GetArticleById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Article,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId, issueNumber = source.IssueId, articleId = source.ArticleId }
                }),
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueController.GetIssueById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Issue,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId, issueNumber = source.IssueId }
                })
            };

            if (!string.IsNullOrWhiteSpace(source.ContentUrl))
            {
                links.Add(new LinkView { Href = source.ContentUrl, Method = "GET", Rel = RelTypes.Download, Accept = MimeTypes.Jpg });
            }

            if (_userHelper.IsWriter(libraryId))
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.UpdateArticleContent),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.Update,
                    MimeType = source.MimeType,
                    Language = source.Language,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId, issueNumber = source.IssueId, articleId = source.ArticleId }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(ArticleController.DeleteArticleContent),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Delete,
                    MimeType = source.MimeType,
                    Language = source.Language,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId, issueNumber = source.IssueId, articleId = source.ArticleId }
                }));
            }

            result.Links = links;
            return result;
        }
    }
}
