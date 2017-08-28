using System.Collections.Generic;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Api.Renderers
{
    public class WordRenderer : RendrerBase, IRenderResponseFromObject<Word, WordView>
    {
        private readonly IUserHelper _userHelper;

        public WordRenderer(
            IRenderLink linkRenderer, 
            IUserHelper userHelper)
            : base(linkRenderer)
        {
            _userHelper = userHelper;
        }

        public WordView Render(Word source)
        {
            var result = source.Map<Word, WordView>();

            var links = new List<LinkView>
                               {
                                   LinkRenderer.Render("GetWordById", "self", new { id = result.Id }),
                                   LinkRenderer.Render("GetWordRelationsById", "relations", new { id = result.Id }),
                                   LinkRenderer.Render("GetWordDetailsById", "details", new { id = result.Id }),
                                   LinkRenderer.Render("GetWordRelationsById", "relationships", new {id = result.Id})
                               };

            if (_userHelper.IsContributor)
            {
                links.Add(LinkRenderer.Render("UpdateWord", "update", new {id = result.Id}));
                links.Add(LinkRenderer.Render("DeleteWord", "delete", new {id = result.Id}));
                links.Add(LinkRenderer.Render("AddWordDetail", "add-detail", new { id = result.Id }));
                links.Add(LinkRenderer.Render("AddRelation", "add-relation", new { id = result.Id }));
            }

            result.Links = links;
            return result;
        }
    }
}