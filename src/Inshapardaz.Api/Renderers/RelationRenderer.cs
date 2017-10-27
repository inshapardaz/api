using System.Collections.Generic;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Api.Renderers
{
    public interface IRenderRelation
    {
        RelationshipView Render(WordRelation source);
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

        public RelationshipView Render(WordRelation source)
        {
            var result = source.Map<WordRelation, RelationshipView>();
            var links = new List<LinkView>
                               {
                                   _linkRenderer.Render("GetRelationById", "self", new { id = source.Id }),
                                   _linkRenderer.Render("GetWordById", "source-word", new { id = source.SourceWordId }),
                                   _linkRenderer.Render("GetWordById", "related-word", new { id = source.RelatedWordId })
                               };

            if (_userHelper.IsContributor)
            {
                links.Add(_linkRenderer.Render("UpdateRelation", "update", new { id = source.Id }));
                links.Add(_linkRenderer.Render("DeleteRelation", "delete", new { id = source.Id }));
            }

            result.Links = links;
            result.RelationType = _enumRenderer.Render(source.RelationType);

            return result;
        }
    }
}