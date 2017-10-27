using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Api.Renderers
{
    public interface IRenderDictionary
    {
        DictionaryView Render(Dictionary source);
    }

    public class DictionaryRenderer : IRenderDictionary
    {
        private readonly string[] _indexes =
        {
            "آ", "ا", "ب", "پ", "ت", "ٹ", "ث", "ج", "چ", "ح", "خ", "د", "ڈ", "ذ", "ر", "ڑ", "ز", "ژ", "س", "ش", "ص",
            "ض", "ط", "ظ", "ع", "غ", "ف", "ق", "ک", "گ", "ل", "م", "ن", "و", "ہ", "ء", "ی"
        };

        private readonly IRenderLink _linkRenderer;
        private readonly IUserHelper _userHelper;

        public DictionaryRenderer(IRenderLink linkRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _userHelper = userHelper;
        }

        public DictionaryView Render(Dictionary source)
        {
            var links = new List<LinkView>
            {
                _linkRenderer.Render("GetDictionaryById", "self", new {id = source.Id}),
                _linkRenderer.Render("GetWords", "index", new {id = source.Id}),
                _linkRenderer.Render("SearchDictionary", "search", new {id = source.Id})
            };

            if (_userHelper.IsContributor)
            {
                links.Add(_linkRenderer.Render("UpdateDictionary", "update", new {id = source.Id}));
                links.Add(_linkRenderer.Render("DeleteDictionary", "delete", new {id = source.Id}));
                links.Add(_linkRenderer.Render("CreateDictionaryDownload", "create-download", new {id = source.Id}));
                links.Add(_linkRenderer.Render("CreateWord", "create-word", new {id = source.Id}));
            }

            if (source.IsPublic)
            {
                links.Add(_linkRenderer.Render("DownloadDictionary", RelTypes.Download, new {id = source.Id, accept = ""}));
            }

            var indexes = new List<LinkView>(_indexes.Select(i => _linkRenderer.Render("GetWordsListStartWith", i,
                new {id = source.Id, startingWith = i})));

            var result = source.Map<Dictionary, DictionaryView>();
            result.Links = links;
            result.Indexes = indexes;
            return result;
        }
    }
}