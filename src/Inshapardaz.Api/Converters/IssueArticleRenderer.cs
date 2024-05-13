using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Api.Converters
{
    public interface IRenderIssueArticle
    {
        IssueArticleContentView Render(IssueArticleContentModel source, int libraryId);

        IssueArticleView Render(IssueArticleModel source, int libraryId, int periodicalId, int volumeNumber, int issueNumber);

        ListView<IssueArticleView> Render(IEnumerable<IssueArticleModel> source, int libraryId, int periodicalId, int volumeNumber, int issueNumber);
    }

    public class IssueArticleRenderer : IRenderIssueArticle
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public IssueArticleRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }

        public ListView<IssueArticleView> Render(IEnumerable<IssueArticleModel> source, int libraryId, int periodicalId, int volumeNumber, int issueNumber)
        {
            var items = source.Select(c => Render(c, libraryId, periodicalId, volumeNumber, issueNumber)).ToList();
            var view = new ListView<IssueArticleView> { Data = items };

            view.Links.Add(_linkRenderer.Render(new Link
            {
                ActionName = nameof(IssueArticleController.GetIssueArticles),
                Method = HttpMethod.Get,
                Rel = RelTypes.Self,
                Parameters = new { libraryId = libraryId, periodicalId = periodicalId, volumeNumber = volumeNumber, issueNumber = issueNumber }
            }));

            if (_userHelper.IsWriter(libraryId))
            {
                view.Links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueArticleController.CreateIssueArticle),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.Create,
                    Parameters = new { libraryId = libraryId, periodicalId = periodicalId, volumeNumber = volumeNumber, issueNumber = issueNumber }
                }));

                view.Links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueArticleController.UpdateIssueArticleSequence),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.ArticleSequence,
                    Parameters = new { libraryId = libraryId, periodicalId = periodicalId, volumeNumber = volumeNumber, issueNumber = issueNumber }
                }));
            }

            return view;
        }

        public IssueArticleView Render(IssueArticleModel source, int libraryId, int periodicalId, int volumeNumber, int issueNumber)
        {
            var result = source.Map();
            var links = new List<LinkView>
            {
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueArticleController.GetIssueArticleById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Self,
                    Parameters = new { libraryId = libraryId, periodicalId = periodicalId, volumeNumber = volumeNumber, issueNumber = issueNumber, sequenceNumber = source.SequenceNumber }
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

            if (source.PreviousArticle != null)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueArticleController.GetIssueArticleById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Previous,
                    Parameters = new { libraryId = libraryId, periodicalId = periodicalId, volumeNumber = volumeNumber, issueNumber = issueNumber, sequenceNumber = source.PreviousArticle.SequenceNumber }
                }));
            }

            if (source.NextArticle != null)
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueArticleController.GetIssueArticleById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Next,
                    Parameters = new { libraryId = libraryId, periodicalId = periodicalId, volumeNumber = volumeNumber, issueNumber = issueNumber, sequenceNumber = source.NextArticle.SequenceNumber }
                }));
            }

            if (_userHelper.IsWriter(libraryId))
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueArticleController.UpdateIssueArticle),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.Update,
                    Parameters = new { libraryId = libraryId, periodicalId = periodicalId, volumeNumber = volumeNumber, issueNumber = issueNumber, sequenceNumber = source.SequenceNumber }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueArticleController.DeleteIssueArticle),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Delete,
                    Parameters = new { libraryId = libraryId, periodicalId = periodicalId, volumeNumber = volumeNumber, issueNumber = issueNumber, sequenceNumber = source.SequenceNumber }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueArticleController.CreateIssueArticleContent),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.AddContent,
                    Parameters = new { libraryId = libraryId, periodicalId = periodicalId, volumeNumber = volumeNumber, issueNumber = issueNumber, sequenceNumber = source.SequenceNumber }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueArticleController.AssignIssueArticleToUser),
                    Method = HttpMethod.Post,
                    Rel = RelTypes.Assign,
                    Parameters = new { libraryId = libraryId, periodicalId = periodicalId, volumeNumber = volumeNumber, issueNumber = issueNumber, sequenceNumber = source.SequenceNumber }
                }));
            }

            if (_userHelper.IsAuthenticated)
            {
                var contents = new List<IssueArticleContentView>();
                foreach (var content in source.Contents)
                {
                    contents.Add(Render(content, libraryId));
                }

                result.Contents = contents;
            }

            result.Links = links;
            return result;
        }

        public IssueArticleContentView Render(IssueArticleContentModel source, int libraryId)
        {
            var result = source.Map();

            var links = new List<LinkView>
            {
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueArticleController.GetIssueArticleContent),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Self,
                    Language = source.Language,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId, volumeNumber = source.VolumeNumber, issueNumber = source.IssueNumber, sequenceNumber = source.SequenceNumber }
                }),
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(PeriodicalController.GetPeriodicalById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Periodical,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId }
                }),
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueController.GetIssueById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Issue,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId, volumeNumber = source.VolumeNumber, issueNumber = source.IssueNumber }
                }),
                _linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueArticleController.GetIssueArticleById),
                    Method = HttpMethod.Get,
                    Rel = RelTypes.Article,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId, volumeNumber = source.VolumeNumber, issueNumber = source.IssueNumber, sequenceNumber = source.SequenceNumber }
                })
            };

            if (_userHelper.IsWriter(libraryId))
            {
                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueArticleController.UpdateIssueArticleContent),
                    Method = HttpMethod.Put,
                    Rel = RelTypes.Update,
                    Language = source.Language,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId, volumeNumber = source.VolumeNumber, issueNumber = source.IssueNumber, sequenceNumber = source.SequenceNumber }
                }));

                links.Add(_linkRenderer.Render(new Link
                {
                    ActionName = nameof(IssueArticleController.DeleteIssueArticleContent),
                    Method = HttpMethod.Delete,
                    Rel = RelTypes.Delete,
                    Language = source.Language,
                    Parameters = new { libraryId = libraryId, periodicalId = source.PeriodicalId, volumeNumber = source.VolumeNumber, issueNumber = source.IssueNumber, sequenceNumber = source.SequenceNumber }
                }));
            }

            result.Links = links;
            return result;
        }
    }
}
