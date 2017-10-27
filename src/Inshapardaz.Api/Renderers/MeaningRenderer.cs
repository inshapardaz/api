using System.Collections.Generic;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Api.Renderers
{
    public interface IRenderMeaning
    {
        MeaningView Render(Meaning source);
    }

    public class MeaningRenderer : IRenderMeaning
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public MeaningRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }

        public MeaningView Render(Meaning source)
        {
            var result = source.Map<Meaning, MeaningView>();
            var links = new List<LinkView>
            {
                _linkRenderer.Render("GetMeaningById", "self", new { id = source.Id }),
                _linkRenderer.Render("GetDetailsById", "worddetail", new { id = source.WordDetailId })
            };
            

            if (_userHelper.IsAuthenticated)
            {
                links.Add(_linkRenderer.Render("UpdateMeaning", "update", new { id = source.Id }));
                links.Add(_linkRenderer.Render("DeleteMeaning", "delete",  new { id = source.Id }));
            }

            result.Links = links;
            return result;
        }
    }
}
