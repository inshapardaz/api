using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Library;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Helpers;
using System.Collections.Generic;

namespace Inshapardaz.Api.Renderers.Library
{
    public interface IRenderPeriodical
    {
        PeriodicalView Render(Periodical source);
    }

    public class PeriodicalRenderer : IRenderPeriodical
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public PeriodicalRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }

        public PeriodicalView Render(Periodical source)
        {
            var result = source.Map<Periodical, PeriodicalView>();
            var links = new List<LinkView>
            {
                _linkRenderer.Render("GetPeriodicalById", RelTypes.Self, new { id = source.Id }),
            };


            if (source.ImageId > 0)
            {
                links.Add(_linkRenderer.Render("GetFileById", RelTypes.Image, new { id = source.ImageId, ext = string.Empty, height = 257, width = 182 }));
            }

            if (_userHelper.IsWriter)
            {
                links.Add(_linkRenderer.Render("UpdatePeriodical", RelTypes.Update, new { id = source.Id }));
                links.Add(_linkRenderer.Render("DeletePeriodical", RelTypes.Delete, new { id = source.Id }));
                links.Add(_linkRenderer.Render("UpdatePeriodicalImage", RelTypes.ImageUpload, new { id = source.Id }));
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
