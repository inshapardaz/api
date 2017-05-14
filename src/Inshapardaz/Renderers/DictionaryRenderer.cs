using System.Collections.Generic;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.Model;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Api.Renderers
{
    public class DictionaryRenderer : RendrerBase, IRenderResponseFromObject<Dictionary, DictionaryView>
    {
        private readonly IUserHelper _userHelper;

        public DictionaryRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
            : base(linkRenderer)
        {
            _userHelper = userHelper;
        }

        public DictionaryView Render(Dictionary source)
        {
            var links = new List<LinkView>
                            {
                                LinkRenderer.Render("GetDictionaryById", "self", new { id = source.Id }),
                                LinkRenderer.Render("GetWords", "index", new { id = source.Id }),
                                LinkRenderer.Render("SearchDictionary", "search", new { id = source.Id })
            };

            if (_userHelper.IsContributor)
            {
                links.Add(LinkRenderer.Render("UpdateDictionary", "update", new { id = source.Id }));
                links.Add(LinkRenderer.Render("DeleteDictionary", "delete", new { id = source.Id }));
                links.Add(LinkRenderer.Render("CreateWord", "create-word", new { id = source.Id }));
            }

            var result = source.Map<Dictionary, DictionaryView>();
            result.Links = links;
            return result;
        }
    }
}