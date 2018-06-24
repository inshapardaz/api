using System.Collections.Generic;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;

namespace Inshapardaz.Api.Renderers.Dictionary
{
    public interface IRenderRelation
    {
        RelationshipView Render(WordRelation source, int dictionaryId, long wordId);
    }

    public class RelationRenderer : IRenderRelation
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IRenderEnum _enumRenderer;
        private readonly IUserHelper _userHelper;

        public RelationRenderer(IRenderLink linkRenderer, IRenderEnum enumRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _enumRenderer = enumRenderer;
            _userHelper = userHelper;
        }

        public RelationshipView Render(WordRelation source, int dictionaryId, long wordId)
        {
            var result = source.Map<WordRelation, RelationshipView>();
            result.SourceWordId = wordId;
            var links = new List<LinkView>
                               {
                                   _linkRenderer.Render("GetRelationById", RelTypes.Self, new { id = dictionaryId, wordId = wordId, relationId = source.Id }),
                                   _linkRenderer.Render("GetWordById", RelTypes.Word, new { id = dictionaryId, wordId = wordId  }),
                                   _linkRenderer.Render("GetWordById", RelTypes.RelatedWord, new { id = dictionaryId, wordId = source.RelatedWordId })
                               };

            if (_userHelper.IsContributor)
            {
                links.Add(_linkRenderer.Render("UpdateRelation", RelTypes.Update, new { id = dictionaryId, wordId = wordId, relationId = source.Id }));
                links.Add(_linkRenderer.Render("DeleteRelation", RelTypes.Delete, new { id = dictionaryId, wordId = wordId, relationId = source.Id }));
            }

            result.Links = links;
            result.RelationType = _enumRenderer.Render(source.RelationType);

            return result;
        }
    }
}