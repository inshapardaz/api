using System.Collections.Generic;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Helpers;

namespace Inshapardaz.Api.Renderers.Dictionary
{
    public interface IRenderRelation
    {
        RelationshipView Render(WordRelation source, int dictionaryId);
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

        public RelationshipView Render(WordRelation source, int dictionaryId)
        {
            var result = source.Map<WordRelation, RelationshipView>();
            var links = new List<LinkView>
                               {
                                   _linkRenderer.Render("GetRelationById", RelTypes.Self, new { id = dictionaryId, wordId = source.SourceWordId, relationId = source.Id }),
                                   _linkRenderer.Render("GetWordById", RelTypes.Word, new { id = dictionaryId, wordId = source.SourceWordId  }),
                                   _linkRenderer.Render("GetWordById", RelTypes.RelatedWord, new { id = dictionaryId, wordId = source.RelatedWordId })
                               };

            if (_userHelper.IsWriter)
            {
                links.Add(_linkRenderer.Render("UpdateRelation", RelTypes.Update, new { id = dictionaryId, wordId = source.SourceWordId, relationId = source.Id }));
                links.Add(_linkRenderer.Render("DeleteRelation", RelTypes.Delete, new { id = dictionaryId, wordId = source.SourceWordId, relationId = source.Id }));
            }

            result.Links = links;
            result.RelationType = _enumRenderer.Render(source.RelationType);

            return result;
        }
    }
}