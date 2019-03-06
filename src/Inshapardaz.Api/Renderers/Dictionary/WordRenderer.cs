using System.Collections.Generic;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Helpers;

namespace Inshapardaz.Api.Renderers.Dictionary
{
    public interface IRenderWord
    {
        WordView Render(Word source, int dictionaryId);
    }

    public class WordRenderer : IRenderWord
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public WordRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }

        public WordView Render(Word source, int dictionaryId)
        {
            var result = source.Map<Word, WordView>();  

            var links = new List<LinkView>
                               {
                                   _linkRenderer.Render("GetWordById", RelTypes.Self, new { id = dictionaryId, wordId = result.Id }),
                                   _linkRenderer.Render("GetWordRelationsById", RelTypes.Relationships, new { id = dictionaryId, wordId = result.Id }),
                                   _linkRenderer.Render("GetWordTranslationsById", RelTypes.Translations, new {id = dictionaryId, wordId = source.Id}),
                                   _linkRenderer.Render("GetWordMeaningByWordId", RelTypes.Meanings, new {id = dictionaryId, wordId = source.Id}),
                                   _linkRenderer.Render("GetDictionaryById", RelTypes.Dictionary, new {id = source.DictionaryId})
                               };

            if (_userHelper.IsWriter)
            {
                links.Add(_linkRenderer.Render("UpdateWord", RelTypes.Update, new { id = dictionaryId, wordId = result.Id }));
                links.Add(_linkRenderer.Render("DeleteWord", RelTypes.Delete, new { id = dictionaryId, wordId = result.Id }));
                links.Add(_linkRenderer.Render("AddTranslation", RelTypes.AddTranslation, new { id = dictionaryId, wordId = source.Id }));
                links.Add(_linkRenderer.Render("AddMeaning", RelTypes.AddMeaning, new { id = dictionaryId, wordId = source.Id }));
                links.Add(_linkRenderer.Render("AddRelation", RelTypes.AddRelation , new { id = dictionaryId, wordId = result.Id }));
            }

            result.Links = links;
            return result;
        }
    }
}