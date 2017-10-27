using System.Collections.Generic;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Api.Renderers
{
    public interface IRenderWord
    {
        WordView Render(Word source, int? dictionaryId);
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

        public WordView Render(Word source, int? dictionaryId)
        {
            var result = source.Map<Word, WordView>();  

            var links = new List<LinkView>
                               {
                                   _linkRenderer.Render("GetWordById", "self", new { id = dictionaryId, wordId = result.Id }),
                                   _linkRenderer.Render("GetWordRelationsById", "relations", new { id = result.Id }),
                                   _linkRenderer.Render("GetWordDetailsById", "details", new { id = result.Id }),
                                   _linkRenderer.Render("GetWordRelationsById", "relationships", new {id = result.Id}),
                                   _linkRenderer.Render("GetDictionaryById", "dictionary", new {id = source.DictionaryId})
                               };

            if (_userHelper.IsContributor)
            {
                links.Add(_linkRenderer.Render("UpdateWord", "update", new { id = dictionaryId, wordId = result.Id }));
                links.Add(_linkRenderer.Render("DeleteWord", "delete", new { id = dictionaryId, wordId = result.Id }));
                links.Add(_linkRenderer.Render("AddWordDetail", "add-detail", new { id = result.Id }));
                links.Add(_linkRenderer.Render("AddRelation", "add-relation", new { id = result.Id }));
            }

            result.Links = links;
            return result;
        }
    }
}