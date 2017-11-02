using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Api.Renderers
{
    public interface IRenderDictionaries
    {
        DictionariesView Render(IEnumerable<Dictionary> source, Dictionary<int, int> wordCounts);
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

        public DictionariesView Render(IEnumerable<Dictionary> source, Dictionary<int, int> wordCounts)
        {
            var links = new List<LinkView>
                            {
                                _linkRenderer.Render("GetDictionaries", RelTypes.Self, null)
                            };

            if (_userHelper.IsContributor)
            {
                links.Add(_linkRenderer.Render("CreateDictionary", RelTypes.Create, null));
            }

            return new DictionariesView
            {
                Links = links,
                Items = source.Select(d => _dictionaryRender.Render(d, wordCounts[d.Id]))
            };
        }
    }
}