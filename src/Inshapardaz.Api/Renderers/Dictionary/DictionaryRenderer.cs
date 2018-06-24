using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Api.View;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain.Helpers;
using ObjectMapper = Inshapardaz.Api.Helpers.ObjectMapper;

namespace Inshapardaz.Api.Renderers.Dictionary
{
    public interface IRenderDictionary
    {
        DictionaryView Render(Domain.Entities.Dictionary.Dictionary source, int wordCount);
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

        public DictionaryView Render(Domain.Entities.Dictionary.Dictionary source, int wordCount)
        {
            var links = new List<LinkView>
            {
                _linkRenderer.Render("GetDictionaryById", RelTypes.Self, new {id = source.Id}),
                _linkRenderer.Render("GetWords", RelTypes.Index, new {id = source.Id}),
                _linkRenderer.Render("SearchDictionary", RelTypes.Search, new {id = source.Id})
            };

            if (_userHelper.IsContributor)
            {
                links.Add(_linkRenderer.Render("UpdateDictionary", RelTypes.Update, new {id = source.Id}));
                links.Add(_linkRenderer.Render("DeleteDictionary", RelTypes.Delete, new {id = source.Id}));
                links.Add(_linkRenderer.Render("CreateDictionaryDownload", RelTypes.CreateDownload, new {id = source.Id}));
                links.Add(_linkRenderer.Render("CreateWord", RelTypes.CreateWord, new {id = source.Id}));
            }

            if (source.Downloads != null && (source.IsPublic || _userHelper.IsContributor))
            {
                foreach (var download in source.Downloads)
                {
                    links.Add(_linkRenderer.Render("DownloadDictionary", RelTypes.Download, download.MimeType, new {id = source.Id, accept = download.MimeType}));
                }
            }

            var indexes = new List<LinkView>(_indexes.Select(i => _linkRenderer.Render("GetWordsListStartWith", i,
                new {id = source.Id, startingWith = i})));

            var result = ObjectMapper.Map<Domain.Entities.Dictionary.Dictionary, DictionaryView>(source);
            result.WordCount = wordCount;
            result.Links = links;
            result.Indexes = indexes;
            return result;
        }
    }
}