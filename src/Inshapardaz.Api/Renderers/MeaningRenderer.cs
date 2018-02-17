using System.Collections.Generic;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Entities;

namespace Inshapardaz.Api.Renderers
{
    public interface IRenderMeaning
    {
        MeaningView Render(Meaning source, int dictionaryId);
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

        public MeaningView Render(Meaning source, int dictionaryId)
        {
            var result = source.Map<Meaning, MeaningView>();
            var links = new List<LinkView>
            {
                _linkRenderer.Render("GetMeaningById", RelTypes.Self, new { id = dictionaryId, meaningId = source.Id }),
                _linkRenderer.Render("GetWordById", RelTypes.Word, new { id= dictionaryId, wordId = source.WordId })
            };
            

            if (_userHelper.IsContributor)
            {
                links.Add(_linkRenderer.Render("UpdateMeaning", RelTypes.Update, new { id = dictionaryId, meaningId = source.Id }));
                links.Add(_linkRenderer.Render("DeleteMeaning", RelTypes.Delete,  new { id = dictionaryId, meaningId = source.Id }));
            }

            result.Links = links;
            return result;
        }
    }
}
