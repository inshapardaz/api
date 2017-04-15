using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Domain.Model;
using Inshapardaz.Helpers;
using Inshapardaz.Model;

namespace Inshapardaz.Renderers
{
    public class RelationsRenderer : RendrerBase, IRenderResponseFromObject<IEnumerable<WordRelation>, RelationshipsView>
    {
        private readonly IRenderResponseFromObject<WordRelation, RelationshipView> _responseRenderer;
        private readonly IUserHelper _userHelper;

        public RelationsRenderer(IRenderLink linkRenderer, IRenderResponseFromObject<WordRelation, RelationshipView> responseRenderer, IUserHelper userHelper)
            : base(linkRenderer)
        {
            _responseRenderer = responseRenderer;
            _userHelper = userHelper;
        }

        public RelationshipsView Render(IEnumerable<WordRelation> source)
        {
            var result = new RelationshipsView {Relationships = source.Select(x => _responseRenderer.Render(x))};
            //var links = new List<LinkView>
            //                   {
            //                       LinkRenderer.Render("GetWordRelationsById", "self", new { id = source.Id }),
            //                       LinkRenderer.Render("GetWordById", "word", new { id = source.RelatedWordId })
            //                   };

            //if (_userHelper.IsContributor)
            //{
            //    links.Add(LinkRenderer.Render("UpdateRelation", "update", new { id = source.RelatedWordId, relatedWith = source.RelatedWordId }));
            //    links.Add(LinkRenderer.Render("DeleteRelation", "delete", new { id = source.RelatedWordId, relatedWith = source.RelatedWordId }));
            //}

            //result.Links = links;
            //result.RelationType = _enumRenderer.Render<RelationType>(source.RelationType);

            return result;
        }
    }
}