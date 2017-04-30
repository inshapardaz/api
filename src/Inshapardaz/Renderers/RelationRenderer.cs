using System.Collections.Generic;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Model;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Api.Renderers
{
    public class RelationRenderer : RendrerBase, IRenderResponseFromObject<WordRelation, RelationshipView>
    {
        private readonly IRenderEnum _enumRenderer;
        private readonly IUserHelper _userHelper;

        public RelationRenderer(IRenderLink linkRenderer, IRenderEnum enumRenderer, IUserHelper userHelper)
            : base(linkRenderer)
        {
            _enumRenderer = enumRenderer;
            _userHelper = userHelper;
        }

        public RelationshipView Render(WordRelation source)
        {
            var result = source.Map<WordRelation, RelationshipView>();
            var links = new  List<LinkView>
                               {
                                   LinkRenderer.Render("GetWordRelationsById", "self", new { id = source.Id }),
                                   LinkRenderer.Render("GetWordById", "word", new { id = source.RelatedWordId })
                               };

            if (_userHelper.IsContributor)
            {
                links.Add(LinkRenderer.Render("UpdateRelation", "update", new { id = source.RelatedWordId, relatedWith = source.RelatedWordId }));    
                links.Add(LinkRenderer.Render("DeleteRelation", "delete", new { id = source.RelatedWordId, relatedWith = source.RelatedWordId }));    
            }

            result.Links = links;
            result.RelationType = _enumRenderer.Render<RelationType>(source.RelationType);

            return result;
        }
    }
}