using System.Collections.Generic;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Api.Renderers
{
    public class MeaningRenderer : RendrerBase, IRenderResponseFromObject<Meaning, MeaningView>
    {
        private readonly IUserHelper _userHelper;

        public MeaningRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
            : base(linkRenderer)
        {
            _userHelper = userHelper;
        }

        public MeaningView Render(Meaning source)
        {
            var result = source.Map<Meaning, MeaningView>();
            var links = new List<LinkView>
            {
                LinkRenderer.Render("GetMeaningById", "self", new { id = source.Id }),
                LinkRenderer.Render("GetDetailsById", "worddetail", new { id = source.WordDetailId })
            };
            

            if (_userHelper.IsAuthenticated)
            {
                links.Add(LinkRenderer.Render("UpdateMeaning", "update", new { id = source.Id }));
                links.Add(LinkRenderer.Render("DeleteMeaning", "delete",  new { id = source.Id }));
            }

            result.Links = links;
            return result;
        }
    }
}
