using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Domain.Model;
using Inshapardaz.Helpers;
using Inshapardaz.Model;

namespace Inshapardaz.Renderers
{
    public class WordDetailRenderer : RendrerBase, 
        IRenderResponseFromObject<WordDetail, WordDetailView>
    {
        private readonly IRenderEnum _enumRenderer;

        private readonly IRenderResponseFromObject<WordDetail, IEnumerable<MeaningContextView>> _meaningRenderer;

        private readonly IRenderResponseFromObject<Translation, TranslationView> _translationRenderer;

        private readonly IUserHelper _userHelper;

        public WordDetailRenderer(
            IRenderLink linkRenderer, 
            IRenderEnum enumRenderer, 
            IRenderResponseFromObject<WordDetail, IEnumerable<MeaningContextView>> meaningRenderer,
            IRenderResponseFromObject<Translation, TranslationView> translationRenderer,
            IUserHelper userHelper)
            : base(linkRenderer)
        {
            _enumRenderer = enumRenderer;
            _meaningRenderer = meaningRenderer;
            _translationRenderer = translationRenderer;
            _userHelper = userHelper;
        }

        public WordDetailView Render(WordDetail source)
        {
            var result = source.Map<WordDetail, WordDetailView>();

            result.Attributes = _enumRenderer.RenderFlags(source.Attributes).Trim(',');
            result.Language = _enumRenderer.Render<Languages>(source.Language);
             var links = new List<LinkView>
                               {
                                   LinkRenderer.Render("GetWordDetailsById", "self", new { id = source.Id }),
                                   LinkRenderer.Render("GetWordById", "word", new { id = source.Id }),
                                   LinkRenderer.Render("GetWordTranslationsById", "translations", new { id = source.Id }),
                                   LinkRenderer.Render("GetWordMeaningById", "meanings", new { id = source.Id })
                               };

            if (_userHelper.IsContributor)
            {
                links.Add(LinkRenderer.Render("UpdateWordDetail", "update", new { id = source.WordInstanceId, wordDetailId = source.Id }));
                links.Add(LinkRenderer.Render("DeleteWordDetail", "delete", new { id = source.WordInstanceId, wordDetailId = source.Id }));
                links.Add(LinkRenderer.Render("AddTranslation", "addTranslation", new { id = source.WordInstanceId, detailId = source.Id }));
                links.Add(LinkRenderer.Render("AddMeaning", "addMeaning", new { id = source.WordInstanceId, detailId = source.Id }));
            }

            result.Links = links;
            result.Meanings = _meaningRenderer.Render(source);
            result.Translations = source.Translations.Select(x => _translationRenderer.Render(x));
            return result;
        }
    }
}
