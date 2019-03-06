﻿using System.Collections.Generic;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Helpers;

namespace Inshapardaz.Api.Renderers.Dictionary
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
                _linkRenderer.Render("GetMeaningById", RelTypes.Self, new { id = dictionaryId, wordId = source.WordId, meaningId = source.Id }),
                _linkRenderer.Render("GetWordById", RelTypes.Word, new { id= dictionaryId, wordId = source.WordId })
            };
            

            if (_userHelper.IsWriter)
            {
                links.Add(_linkRenderer.Render("UpdateMeaning", RelTypes.Update, new { id = dictionaryId, wordId = source.WordId, meaningId = source.Id }));
                links.Add(_linkRenderer.Render("DeleteMeaning", RelTypes.Delete,  new { id = dictionaryId, wordId = source.WordId, meaningId = source.Id }));
            }

            result.Links = links;
            return result;
        }
    }
}
