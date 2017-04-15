using System;
using System.Collections.Generic;
using System.Linq;

using Inshapardaz.Domain.Model;
using Inshapardaz.Model;
using Inshapardaz.Helpers;

namespace Inshapardaz.Renderers
{
    public class DictionariesRenderer : RendrerBase, IRenderResponseFromObject<IEnumerable<Dictionary>, DictionariesView>
    {
        private readonly IUserHelper _userHelper;
        private readonly IRenderResponseFromObject<Dictionary, DictionaryView> _dictionaryRenderer;
        public DictionariesRenderer(IRenderLink linkRenderer, 
                    IUserHelper userHelper, 
                    IRenderResponseFromObject<Dictionary, DictionaryView> dictionaryRenderer)
            : base(linkRenderer)
        {
            _userHelper = userHelper;
            _dictionaryRenderer = dictionaryRenderer;
        }

        public DictionariesView Render(IEnumerable<Dictionary> source)
        {
            var links = new List<LinkView>
                            {
                                LinkRenderer.Render("DictionaryIndex", "self", null)
                            };

            if (_userHelper.IsContributor)
            {
                links.Add(LinkRenderer.Render("CreateDictioanry", "create", null));
            }

            return new DictionariesView()
            {
                Links = links,
                Items = source.Select(d => _dictionaryRenderer.Render(d))
            };
        }
    }
}