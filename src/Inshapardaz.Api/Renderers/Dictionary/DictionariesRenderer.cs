using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Helpers;

namespace Inshapardaz.Api.Renderers.Dictionary
{
    public interface IRenderDictionaries
    {
        DictionariesView Render(IEnumerable<Domain.Entities.Dictionary.Dictionary> source);
    }

    public class DictionariesRenderer : IRenderDictionaries
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;
        private readonly IRenderDictionary _dictionaryRender;

        public DictionariesRenderer(IRenderLink linkRenderer, IUserHelper userHelper, IRenderDictionary dictionaryRender)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
            _dictionaryRender = dictionaryRender;
        }

        public DictionariesView Render(IEnumerable<Domain.Entities.Dictionary.Dictionary> source)
        {
            var links = new List<LinkView>
                            {
                                _linkRenderer.Render("GetDictionaries", RelTypes.Self, null)
                            };

            if (_userHelper.IsWriter)
            {
                links.Add(_linkRenderer.Render("CreateDictionary", RelTypes.Create, null));
            }

            return new DictionariesView
            {
                Links = links,
                Items = source.Select(d => _dictionaryRender.Render(d)).ToList()
            };
        }
    }
}