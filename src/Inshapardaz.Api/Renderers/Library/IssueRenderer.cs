using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;
using System.Collections.Generic;

namespace Inshapardaz.Api.Renderers.Library
{
    public interface IRenderIssue
    {
        IssueView Render(Issue source);
    }

    public class IssueRenderer : IRenderIssue
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public IssueRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }

        public IssueView Render(Issue source)
        {
            var result = source.Map<Issue, IssueView>();
            var links = new List<LinkView>
            {
                _linkRenderer.Render("GetPeriodicalById", RelTypes.Self, new { id = source.PeriodicalId }),
            };


            if (source.ImageId > 0)
            {
                links.Add(_linkRenderer.Render("GetFileById", RelTypes.Image, new { id = source.ImageId, ext = string.Empty, height = 257, width = 182 }));
            }

            if (_userHelper.IsWriter)
            {
                links.Add(_linkRenderer.Render("UpdateIssue", RelTypes.Update, new { id = source.PeriodicalId, issueId = source.Id }));
                links.Add(_linkRenderer.Render("DeleteIssue", RelTypes.Delete, new { id = source.PeriodicalId, issueId = source.Id }));
                links.Add(_linkRenderer.Render("UpdateIssueImage", RelTypes.ImageUpload, new { id = source.PeriodicalId, issueId = source.Id }));
            }

            result.Links = links;

            //if (source.Category != null)
            //{
            //    var categories = new List<CategoryView>();
            //    foreach (var category in source.Categories)
            //    {
            //        categories.Add(_categoryRenderer.RenderResult(category));
            //    }

            //    result.Categories = categories;
            //}

            return result;
        }
    }
}
