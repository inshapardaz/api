using System.Collections.Generic;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Entities;

namespace Inshapardaz.Api.Renderers
{
    public interface IRenderTranslation
    {
        TranslationView Render(Translation source, int dictionaryId);
    }

    public class TranslationRenderer : IRenderTranslation
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IRenderEnum _enumRenderer;
        private readonly IUserHelper _userHelper;

        public TranslationRenderer(IRenderLink linkRenderer,
            IRenderEnum enumRenderer,
            IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _enumRenderer = enumRenderer;
            _userHelper = userHelper;
        }

        public TranslationView Render(Translation source, int dictionaryId)
        {
            var result = source.Map<Translation, TranslationView>();

            result.Language = _enumRenderer.Render(source.Language);

            var links = new List<LinkView>
            {
                _linkRenderer.Render("GetTranslationById", RelTypes.Self, new { id = dictionaryId,  wordId = source.WordId , translationId = source.Id }),
                _linkRenderer.Render("GetWordById", RelTypes.Word, new { id= dictionaryId, wordId = source.WordId })
            };

            if (_userHelper.IsContributor)
            {
                links.Add(_linkRenderer.Render("UpdateTranslation", RelTypes.Update, new { id = dictionaryId, translationId = source.Id }));
                links.Add(_linkRenderer.Render("DeleteTranslation", RelTypes.Delete, new { id = dictionaryId, translationId = source.Id }));
            }
            result.Links = links;
            return result;
        }
    }
}