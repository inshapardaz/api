using System.Collections.Generic;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Api.Renderers
{
    public class WordIndexRenderer : RendrerBase, IRenderResponseFromObject<Word, WordView>
    {
        private readonly IUserHelper _userHelper;

        public WordIndexRenderer(IRenderLink linkrenderer, IUserHelper userHelper)
            : base(linkrenderer)
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
                LinkRenderer.Render("GetWordDetailsById", "details", new { id = result.Id })
            };

            if (_userHelper.IsAuthenticated)
            {
                links.Add(LinkRenderer.Render("UpdateWord", "update", new { id = result.Id }));
                links.Add(LinkRenderer.Render("DeleteWord", "delete", new { id = result.Id }));
                links.Add(LinkRenderer.Render("AddRelation", "addRelate", new { id = result.Id }));
                //links.Add(LinkRenderer.Render("MergeWords", "merge", new {id = result.Id }));
            }

            result.Links = links;

            return result;
        }
    }
}