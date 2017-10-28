using System.Collections.Generic;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Api.Renderers
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
                                   _linkRenderer.Render("GetWordDetailsById", RelTypes.WordDetails, new { id = dictionaryId, wordId = result.Id }),
                                   _linkRenderer.Render("GetDictionaryById", RelTypes.Dictionary, new {id = source.DictionaryId})
                               };

            if (_userHelper.IsContributor)
            {
                links.Add(_linkRenderer.Render("UpdateWord", RelTypes.Update, new { id = dictionaryId, wordId = result.Id }));
                links.Add(_linkRenderer.Render("DeleteWord", RelTypes.Delete, new { id = dictionaryId, wordId = result.Id }));
                links.Add(_linkRenderer.Render("AddWordDetail", RelTypes.AddDetail, new { id = dictionaryId, wordId = result.Id }));
                links.Add(_linkRenderer.Render("AddRelation", RelTypes.AddRelation , new { id = dictionaryId, wordId = result.Id }));
            }

            result.Links = links;
            return result;
        }
    }
}